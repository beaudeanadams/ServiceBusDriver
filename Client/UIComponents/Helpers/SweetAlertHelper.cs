using System;
using System.Threading.Tasks;
using CurrieTechnologies.Razor.SweetAlert2;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Helpers
{
    public class SweetAlertHelper
    {
        private const string DefaultTitle = "Are you sure?";
        private const string DefaultText = "This operation is not reversible";
        private const string DefaultConfirmButtonText = "Yes, Continue";
        private const string DefaultCancelButtonText = "My bad! Cancel";

        public delegate Task HandleAlertResult(SweetAlertResult result);

        public async Task ShowSweetAlertConfirm(SweetAlertService swal,
                                                       string title,
                                                       string text,
                                                       bool cancelButton,
                                                       string confirmButtonText,
                                                       string cancelButtonText, HandleAlertResult handleResult)
        {
            title = title.IsNullOrWhiteSpace() ? DefaultTitle : title;
            text = text.IsNullOrWhiteSpace() ? DefaultText : text;
            confirmButtonText = confirmButtonText.IsNullOrWhiteSpace() ? DefaultConfirmButtonText : confirmButtonText;
            cancelButtonText = cancelButtonText.IsNullOrWhiteSpace() ? DefaultCancelButtonText : cancelButtonText;

            var result = await swal.FireAsync(new SweetAlertOptions
            {
                Title = title,
                Text = text,
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = cancelButton,
                ConfirmButtonText = confirmButtonText,
                CancelButtonText = cancelButtonText,
                ConfirmButtonColor = UiConstants.PrimaryColor,
                CancelButtonColor = UiConstants.ColorRed400,
                
            });

            await handleResult(result);
        }
    }
}