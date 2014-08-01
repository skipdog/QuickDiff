using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Helpers
{
    public class NotificationObject : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Constructor
        public NotificationObject()
        {
            LastExceptionMessage = new ObservableCollection<string>();
        }
        #endregion

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region LastExceptionMessage
        private ObservableCollection<string> lastExceptionMessage;
        public ObservableCollection<string> LastExceptionMessage 
        {
            get { return lastExceptionMessage; }
            set
            {
                lastExceptionMessage = value;
                RaisePropertyChanged(() => LastExceptionMessage);
            }
        }
        #endregion

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return this.GetValidationError(columnName); }
        }

        public string[] GetValidatedProperties()
        {
            return OnGetValidatedProperties();
        }

        protected virtual string[] OnGetValidatedProperties()
        {
            return null;
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    foreach (string property in GetValidatedProperties())
                        if (GetValidationError(property) != null)
                            return false;
                }
                catch { }
                return true;
            }
        }

        public string GetValidationError(string propertyName)
        {
            return OnGetValidationError(propertyName);
        }

        protected virtual string OnGetValidationError(string propertyName)
        {
            return null;
        }

    }
}
