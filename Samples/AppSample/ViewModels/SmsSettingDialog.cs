﻿using AppSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VagabondK;

namespace AppSample.ViewModels
{
    [ServiceDescription]
    public class SmsSettingDialog : NotifyPropertyChangeObject, INotifyLoaded, IQueryClosing
    {
        public SmsSettingDialog(IFocusHandler focusHandler, IMessageBox messageBox)
        {
            this.focusHandler = focusHandler;
            this.messageBox = messageBox;
        }

        private readonly IFocusHandler focusHandler;
        private readonly IMessageBox messageBox;

        public SmsSetting SmsSetting { get => Get(() => new SmsSetting()); }

        public void OnLoaded()
        {
            focusHandler.Focus(nameof(SmsSetting.ServiceID));
        }

        public async Task<bool> QueryClosing(bool result)
        {
            return !(result && await IsNullOrWhiteSpace(SmsSetting,
                nameof(SmsSetting.ServiceID),
                nameof(SmsSetting.AccessKeyID),
                nameof(SmsSetting.SecretKey),
                nameof(SmsSetting.SenderPhoneNumber)));
        }

        private async Task<bool> IsNullOrWhiteSpace(object source, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                if (source.GetType().GetProperty(propertyName)?.GetValue(source) is string value
                    && string.IsNullOrWhiteSpace(value))
                {
                    focusHandler.Focus(propertyName);
                    await messageBox.Show($"{propertyName} 누락.", "입력 오류", MessageBoxButton.OK, MessageImage.Error);
                    return true;
                }
            return false;
        }
    }
}
