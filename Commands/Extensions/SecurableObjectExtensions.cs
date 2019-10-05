using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharePointPnP.PowerShell.Commands.Extensions
{
    public static class SecurableObjectExtensions
    {
        public static bool CanSetPermissions(this SecurableObject secObj)
        {
            return secObj.IsPropertyAvailable("HasUniqueRoleAssignments");
        }

        private static T Cast<T>(dynamic o) => (T)o;

        private static ValueTuple<string, object> GetNameAndIdValue(SecurableObject secObj, string nameProperty)
        {
            Type objType = secObj.GetType();
            PropertyInfo namePi = objType.GetProperty(nameProperty, BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo idPi = objType.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
            string nameVal = namePi.GetValue(secObj) as string;
            object idVal = idPi.GetValue(secObj);
            return new ValueTuple<string, object>(nameVal, idVal);
        }

        public static PermissionCollection GetPermissions(this SecurableObject secObj)
        {
            Type secType = secObj.GetType();
            string name = secType.Name.Equals("ListItem") ? "DisplayName" : "Title";

            secObj.Context.Load(secObj, s => s.HasUniqueRoleAssignments, s => s.RoleAssignments);
            try
            {
                secObj.Context.ExecuteQuery();
            }
            catch (ServerException)
            {
                return null;
            }

            bool? check = secObj.IsPropertyAvailable("HasUniqueRoleAssignments")
                   ? (bool?)secObj.HasUniqueRoleAssignments
                   : null;

            if (!check.HasValue)
                return null;

            //if (!(secObj is Web))
            //{
            //    MethodInfo genMeth = typeof(ClientObjectExtensions)
            //        .GetMethod("GetClientObjectExpression", BindingFlags.Static | BindingFlags.NonPublic)
            //            .MakeGenericMethod(secType);

            //    object nameExpression = genMeth.Invoke(null, new object[]
            //    {
            //        secObj, name
            //    });
            //    object idExpression = genMeth.Invoke(null, new object[]
            //    {
            //        secObj, "Id"
            //    });

            //    MethodInfo genLoad = typeof(ClientObjectExtensions)
            //        .GetMethod("SpecialLoad", BindingFlags.NonPublic | BindingFlags.Static)
            //            .MakeGenericMethod(secType);

            //    genLoad.Invoke(secObj, new object[]
            //    {
            //        secObj, nameExpression
            //    });
            //    genLoad.Invoke(secObj, new object[]
            //    {
            //        secObj, idExpression
            //    });
            //    secObj.Context.ExecuteQuery();
            //}
            PermissionCollection permissions = PermissionCollection.ResolvePermissions(secObj);
            //ValueTuple<string, object> tuple = GetNameAndIdValue(secObj, name);
            return permissions;
        }

#if DEBUG

        public static object TestGetPermissions(SecurableObject secObj)
        {
            return secObj.GetPermissions();
        }

#endif
    }
}
