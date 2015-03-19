using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Nuclear.Tracing.API.SystemInfo
{
    /// <summary>
    /// Класс сбора информации о настройка безопасности
    /// </summary>
    public static class SecurityInfo
    {
        /// <summary>
        /// Возвращает инфу по настройках безопасности текущего активного пользователя Windows
        /// Название, есть или нет админ.привелегии, в каких группах состоит
        /// </summary>
        public static String UserSecuritySettingsDescription
        {
            get 
            {
                var sb = new StringBuilder();
                try
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    if (identity != null)
                    {
                        sb.AppendLine(string.Format("User identity = {0}", identity.Name));
                        var principal = new WindowsPrincipal(identity);
                        sb.AppendLine(string.Format("Current identity {0} administrator privileges",
                                                  principal.IsInRole(WindowsBuiltInRole.Administrator) ? "HAS" : "has NOT"));
                        var groups = identity.Groups;
                        if (groups != null && groups.Count > 0)
                        {
                            sb.AppendLine("User is member of groups: ");
                            foreach (var group in groups)
                            {
                                var result = group.Value;
                                try
                                {
                                    if (group is SecurityIdentifier)
                                    {
                                        var ntAccount = group.Translate(typeof(NTAccount)) as NTAccount;
                                        if (ntAccount != null)
                                        {
                                            result += " = " + ntAccount;
                                        }
                                    }
                                    else if (group is NTAccount)
                                    {
                                        result = group.ToString();
                                    }
                                }
                                catch (Exception)
                                {
                                    sb.AppendLine("Can't get extended info for group: " + result);
                                }

                                sb.AppendLine(result);
                            }
                        }
                        else
                        {
                            sb.AppendLine("Can't detect user groups");
                        }
                    }
                }
                catch (Exception)
                {
                    sb.AppendLine("Can't detect user security info");
                }

                return sb.ToString();
            }
        }

        private static Dictionary<string, int> ExtractRights(FileSystemRights rights)
        {
            return (from name in Enum.GetNames(typeof(FileSystemRights)) 
                        let val = (int)Enum.Parse(typeof(FileSystemRights), name)
                        let enumVal = (FileSystemRights)val
                        where ((enumVal & rights) == enumVal)
                        select new { val, name }).ToDictionary(i => i.name, i => i.val);
        }

        private static FileSystemRights GetRights(WindowsIdentity user, AuthorizationRuleCollection fileSystemObjectRules)
        {
            IEnumerable<AuthorizationRule> userRules = from AuthorizationRule rule in fileSystemObjectRules
                                                       where user.Groups != null && (user.User != null && (user.User.Equals(rule.IdentityReference)
                                                                                                           || user.Groups.Contains(rule.IdentityReference)))
                                                       select rule; 
            FileSystemRights denyRights = 0;
            FileSystemRights allowRights = 0;
            // iterates on rules to compute denyRights and allowRights  
            foreach (FileSystemAccessRule rule in userRules)
            {
                if (rule.AccessControlType.Equals(AccessControlType.Deny))
                {
                    denyRights = denyRights | rule.FileSystemRights;
                }
                else if (rule.AccessControlType.Equals(AccessControlType.Allow))
                {
                    allowRights = allowRights | rule.FileSystemRights;
                }
            }
            // allowRights = allowRights - denyRights  
            allowRights = allowRights & ~denyRights;
            // return effective rights
            return allowRights;
        }

        private static FileSystemRights? GetRightsOnFile(WindowsIdentity user, string fileFullPath)
        {
            try
            {
                if (!File.Exists(fileFullPath))
                {
                    return null;
                }

                var acl = File.GetAccessControl(fileFullPath).GetAccessRules(true, true, typeof(SecurityIdentifier));
                return GetRights(user, acl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static FileSystemRights? GetRightsOnDirectory(WindowsIdentity user, string directoryFullPath)
        {
            try
            {
                if (!Directory.Exists(directoryFullPath))
                {
                    return null;
                }

                var acl = Directory.GetAccessControl(directoryFullPath).GetAccessRules(true, true, typeof(SecurityIdentifier));
                return GetRights(user, acl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает строковое описание действующих привелегий безопасности для WindowsIdentity.GetCurrent на указанный файл
        /// </summary>
        /// <param name="fileFullPath">Полный путь к файлу для которого определяются разрешения</param>
        public static String GetFileEffectiveRightsDescription(string fileFullPath)
        {
            const String DefaultError = "Can't detect rights";
            WindowsIdentity identity = null;

            try
            {
                identity = WindowsIdentity.GetCurrent();
            }
            catch (Exception)
            {
            }

            if (identity == null)
            {
                return DefaultError;
            }

            var rights = GetEffectiveRightsOnFile(identity, fileFullPath);
            if (rights == null || rights.Count <= 0)
            {
                return DefaultError;
            }

            var sb = new StringBuilder();
            foreach (var right in rights)
            {
                sb.AppendLine(right.Key);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает действующие привелегии безопасности для указанного пользователя на указанный файл
        /// </summary>
        /// <param name="user">Пользователь для которого определяются привелегии</param>
        /// <param name="fileFullPath">Полный путь к файлу для которого определяются разрешения</param>
        /// <returns>Пары - имя разрешения и значение enum FileSystemRights</returns>
        public static Dictionary<string, int> GetEffectiveRightsOnFile(WindowsIdentity user, string fileFullPath)
        {
            var rights = GetRightsOnFile(user, fileFullPath);
            if (rights == null)
            {
                return null;
            }

            return ExtractRights(rights.Value);
        }

        /// <summary>
        /// Возвращает строковое описание действующих привелегий безопасности для WindowsIdentity.GetCurrent на указанную директорию
        /// </summary>
        /// <param name="directoryFullPath">Полный путь к директории для которой определяются разрешения</param>
        public static String GetDirectoryEffectiveRightsDescription(string directoryFullPath)
        {
            const String DefaultError = "Can't detect rights";
            WindowsIdentity identity = null;

            try
            {
                identity = WindowsIdentity.GetCurrent();
            }
            catch (Exception)
            {
            }

            if (identity == null)
            {
                return DefaultError;
            }

            var rights = GetEffectiveRightsOnDirectory(identity, directoryFullPath);
            if (rights == null || rights.Count <= 0)
            {
                return DefaultError;
            }

            var sb = new StringBuilder();
            foreach (var right in rights)
            {
                sb.AppendLine(right.Key);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает действующие привелегии безопасности для указанного пользователя на указанную директорию
        /// </summary>
        /// <param name="user">Пользователь для которого определяются привелегии</param>
        /// <param name="directoryFullPath">Полный путь к директории для которой определяются разрешения</param>
        /// <returns>Пары - имя разрешения и значение enum FileSystemRights</returns>
        public static Dictionary<string, int> GetEffectiveRightsOnDirectory(WindowsIdentity user, string directoryFullPath)
        {
            var rights = GetRightsOnDirectory(user, directoryFullPath);
            if (rights == null)
            {
                return null;
            }

            return ExtractRights(rights.Value);
        }

        /// <summary>
        /// Проверяет обладает ли достаточными привелегиями безопасности указанный пользователь на указанную директорию
        /// </summary>
        /// <param name="user">Пользователь для которого определяются привелегии</param>
        /// <param name="directoryFullPath">Полный путь к директории для которой определяются разрешения</param>
        /// <param name="expectedRights">Наличие этих прав нужно проверить</param>
        /// <returns>True - если указанные привелегии есть</returns>
        public static bool CheckAccessForDirectory(WindowsIdentity user, string directoryFullPath, FileSystemRights expectedRights)
        {
            var allowRights = GetRightsOnDirectory(user, directoryFullPath);
            return (allowRights & expectedRights) == expectedRights;
        }

        /// <summary>
        /// Проверяет обладает ли достаточными привелегиями безопасности указанный пользователь на указанный файл
        /// </summary>
        /// <param name="user">Пользователь для которого определяются привелегии</param>
        /// <param name="fileFullPath">Полный путь к файлу для которого определяются разрешения</param>
        /// <param name="expectedRights">Наличие этих прав нужно проверить</param>
        /// <returns>True - если указанные привелегии есть</returns>
        public static bool CheckAccessForFile(WindowsIdentity user, string fileFullPath, FileSystemRights expectedRights)
        {
            var allowRights = GetRightsOnFile(user, fileFullPath);
            return (allowRights & expectedRights) == expectedRights;
        }
    }
}
