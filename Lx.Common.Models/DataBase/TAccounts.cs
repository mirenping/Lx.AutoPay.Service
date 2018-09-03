using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.DataBase
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public class TAccounts
    {
        private int fID;
        public int FID
        {
            get { return fID; }
            set { fID = value; }
        }

        private string fAccount;
        public string FAccount
        {
            get { return fAccount; }
            set { fAccount = value; }
        }

        private string fLogonPass;
        public string FLogonPass
        {
            get { return fLogonPass; }
            set { fLogonPass = value; }
        }
    }
}
