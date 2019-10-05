using Microsoft.SharePoint.Client.Utilities;
using SharePointPnP.PowerShell.Commands.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.SharePoint.Client
{
    public class Permission
    {
        #region PROPERTIES
        public ClientRuntimeContext Context { get; set; }
        public string LoginName { get; set; }
        public object MemberId { get; set; }
        public string MemberName { get; set; }
        public SecurableObject SecuringObject { get; set; }
        //public string Object { get; set; }
        //public object ObjectId { get; set; }
        public string[] Permissions { get; set; }
        public int PrincipalId { get; set; }
        public PrincipalType Type { get; set; }

        #endregion

        #region CONSTRUCTORS
        public Permission(RoleAssignment ass, bool andLoad = true)
        {
            this.Context = ass.Context;
            if (andLoad)
                ass.LoadAssignment();

            this.LoginName = ass.Member.LoginName;
            this.MemberName = ass.Member.Title;
            this.MemberId = ass.Member.Id;
            this.PrincipalId = ass.PrincipalId;
            this.Type = ass.Member.PrincipalType;
            this.Permissions = this.ParseBindings(ass.RoleDefinitionBindings);
        }

        #endregion

        #region PUBLIC METHODS


        #endregion

        #region BACKEND/PRIVATE METHODS
        public string[] ParseBindings(RoleDefinitionBindingCollection bindingCol)
        {
            string[] strPerms = new string[bindingCol.Count];
            for (int i = 0; i < bindingCol.Count; i++)
            {
                RoleDefinition bind = bindingCol[i];
                strPerms[i] = bind.Name;
            }
            return strPerms;
        }

        #endregion
    }
}