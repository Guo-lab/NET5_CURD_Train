
using System.Collections.Generic;

namespace ProjectBase.Utils
{
    public class ProjectHierarchy
    {
        private static string _businessDelegateNS;
        private static string _domainNS;
        private static string _controllerNS;
        private static string _viewModelNS;

        private static string[] _iocCheckNS;

        public static string ProjectName = "App";

        public static IDictionary<string, string> NamespaceMapToTablePrefix { get; set; }
        public static IEnumerable<string> NonprefixMvcModuleNames { get; set; }

        public static string BusinessDelegateNS
        {
            get
            {
                if (string.IsNullOrEmpty(_businessDelegateNS))
                    return ProjectName + ".BusinessDelegate";
                else
                {
                    return _businessDelegateNS;
                }
            }
            set { _businessDelegateNS = value; }

        }
        public static string DomainNS
        {
            get
            {
                if (string.IsNullOrEmpty(_domainNS))
                    return ProjectName + ".Domain";
                else
                {
                    return _domainNS;
                }
            }
            set { _domainNS = value; }

        }
        public static string ControllerNS
        {
            get
            {
                if (string.IsNullOrEmpty(_controllerNS))
                    return ProjectName + ".Web.Mvc";
                else
                {
                    return _controllerNS;
                }
            }
            set { _controllerNS = value; }

        }
        public static string ViewModelNS
        {
            get
            {
                if (string.IsNullOrEmpty(_viewModelNS))
                    return ProjectName + ".Common";
                else
                {
                    return _viewModelNS;
                }
            }
            set
            {
                _viewModelNS = value;
            }
        }
        public static string[] IocCheckNS
        {
            get
            {
                if (_iocCheckNS == null)
                {
                    _iocCheckNS = new string[] { ProjectName };
                }
                return _iocCheckNS;
            }
            set
            {
                _iocCheckNS = value;
            }
        }

        public static string MessagesResourceClassKey = "Messages";
        public static string ValidationMessagesResourceClassKey = "ValidationMessages";
        public static string DisplayNameResourceClassKey = "DisplayName";
    }
}
