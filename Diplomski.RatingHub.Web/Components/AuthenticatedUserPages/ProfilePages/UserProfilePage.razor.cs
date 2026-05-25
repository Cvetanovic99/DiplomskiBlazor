using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.ProfilePages;

public partial class UserProfilePage
{
    [Inject] ICurrentUserService CurrentUserService { get; set; } = null!;
    [Inject] public IUserProfileDataService UserProfileDataService { get; set; } = null!;
    [Inject] public IAccountDataService AccountDataService { get; set; } = null!;
    [Inject] public IHttpService HttpService { get; set; } = null!;
    
    
    private AuthenticatedUserDto _authenticatedUser;
    
    private UserProfileDto _userProfile;
    private ChangeUserPasswordDto _changePasswordModel = new ChangeUserPasswordDto();
    
    public const string _userProfileImageUrl = "user-image";
    private string _originalImage;
    private string _newImage;
    private bool _imageChanged;

    private string _confirmPassword = "";
    
    protected override async Task OnInitializedAsync()
    {
        await GetCurrentUser();
        await LoadUserData();
        
        _originalImage = _userProfile.ProfileImagePath;
    }
    
    private async Task LoadUserData()
    {
        var res = await InvokeDataServiceMethod(
            () => UserProfileDataService.GetUserProfile(_authenticatedUser.IdentityId),
            errorMessage: "Greška pri učitavanju");

        if (!res.ExceptionOccurred)
        {
            _userProfile = res.Result;
        }
    }
    
    
    private async Task GetCurrentUser()
    {
        var currentUser = await CurrentUserService.GetAuthenticatedUserAsync();
        if (currentUser == null)
        {
            ShowNotification("Doslo je do greske prilikom ucitavanja korisnika", NotificationSeverity.Error);
            return;
        }
        _authenticatedUser = currentUser;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnChangePasswordSubmit(ChangeUserPasswordDto model)
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da promenite sifru","Promena sifre",
            new ConfirmOptions { OkButtonText = "Promeni", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            _changePasswordModel.UserIdentityId = _authenticatedUser.IdentityId;
        
            var response = await InvokeDataServiceMethod(
                () => AccountDataService.ChangeUserPassword(_changePasswordModel));

            if (response)
            {
                ShowNotification("Uspesno ste promenili sifru", NotificationSeverity.Success);
                _changePasswordModel.OldPassword = "";
                _changePasswordModel.NewPassword = "";
                _confirmPassword = "";
                StateHasChanged();
            }
        }
    }
    
    public async Task OnEditUserDataSubmit(UserProfileDto model)
    {
        if (_imageChanged)
        {
            // obriši staru
            if (!string.IsNullOrEmpty(_originalImage))
            {
                await HttpService.DeleteImage(_userProfileImageUrl, _originalImage);
            }

            model.ProfileImagePath = _newImage;
        }

        var res = await InvokeDataServiceMethod(
            () => UserProfileDataService.EditUserProfile(model),
            errorMessage: "Doslo je do greske prilikom azuriranja podataka");

        if (!res.ExceptionOccurred)
        {
            _userProfile = res.Result;
            _originalImage = _userProfile.ProfileImagePath;
            _newImage = null;
            _imageChanged = false;

            ShowNotification("Uspesno ste azurirali podatke", NotificationSeverity.Success);
        }
    }

    public async Task OnDeleteUserProfileClicked()
    {
        if (_userProfile.DoesOwnCompanies)
        {
            await DialogService.Alert(
                "Prvo morate obrisati sve kompanije koje ste kreirali. Nakon toga mozete obrisati i vas profil", "Obavestenje", 
                new AlertOptions(){OkButtonText = "Ok", ShowClose = false});

            return;
        }

        var res = await DialogService.Confirm("Da li ste sigurni da zelite da izbrisete ovaj profil?","Brisanje profila",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
        
            var response = await InvokeDataServiceMethod(
                () => AccountDataService.DeleteUserProfile(_authenticatedUser.IdentityId, _userProfile.Id),
                errorMessage: "Doslo je do greske prilikom brisanja profila");

            if (response)
            {
                await DialogService.OpenAsync<LogoutAfterProfileDeleteDialog>(
                    "Brisanje profila",
                    options: new DialogOptions
                    {
                        Width = "500px",
                        Height = "auto",
                        Style = "margin-top: 130px",
                        ShowClose = false
                    });
            }
        }
    }
    
    private string GetCurrentImage()
    {
        if (!string.IsNullOrEmpty(_newImage))
            return _newImage;

        if (!string.IsNullOrEmpty(_userProfile?.ProfileImagePath))
            return _userProfile.ProfileImagePath;

        return "/images/userProfileImages/universalProfileImage.svg";
    }

    private async Task HandleImage(InputFileChangeEventArgs e)
    {
        var file = e.File;

        var content = new MultipartFormDataContent();
        var stream = file.OpenReadStream(5_000_000);

        content.Add(new StreamContent(stream), "file", file.Name);

        var response = await HttpService.UploadImage(content, _userProfileImageUrl);

        if (!response.ExceptionOccurred)
        {
            // obriši prethodnu novu ako postoji
            if (!string.IsNullOrEmpty(_newImage))
            {
                await HttpService.DeleteImage(_userProfileImageUrl, _newImage);
            }

            _newImage = response.Result.Path;
            _imageChanged = true;

            StateHasChanged();
        }
    }
    
    private void RemoveNewImage()
    {
        _newImage = null;
        _imageChanged = true;
    }
    
    private async Task CancelChanges()
    {
        if (!string.IsNullOrEmpty(_newImage))
        {
            await HttpService.DeleteImage(_userProfileImageUrl, _newImage);
        }

        _newImage = null;
        _imageChanged = false;

        await LoadUserData();
    }
}

public class ChangeUserPasswordDto 
{
    public string UserIdentityId { get; set; }
    public string OldPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}