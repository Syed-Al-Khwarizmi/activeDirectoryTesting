using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.IO;

namespace ConsoleApplication1
{

    public static class Methods
    {
        public static T ldap_get_value<T>(PropertyValueCollection property)
        {
            object value = null;
            foreach (object tmpValue in property) value = tmpValue;
            return (T)value;
        }

        public static string ldap_get_domainname(DirectoryEntry entry)
        {
            if (entry == null || entry.Parent == null) return null;
            using (DirectoryEntry parent = entry.Parent)
            {
                if (ldap_get_value<string>(parent.Properties["objectClass"]) == "domainDNS")
                    return ldap_get_value<string>(parent.Properties["dc"]);
                else
                    return ldap_get_domainname(parent);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string[] _properties = new string[] { "objectClass", "distinguishedName", "samAccountName", "userPrincipalName", "displayName", "mail", "title", "company", "thumbnailPhoto", "useraccountcontrol" };
            string account = "my-user-name";
            // OR even better:
            // string account = "my-user-name@DOMAIN.local";

            using (DirectoryEntry ldap = new DirectoryEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(ldap))
                {
                    searcher.PropertiesToLoad.AddRange(_properties);
                    if (account.Contains('@')) searcher.Filter = "(userPrincipalName=" + account + ")";
                    else searcher.Filter = "(samAccountName=" + account + ")";
                    var user = searcher.FindOne().GetDirectoryEntry();
                    StreamWriter var = new StreamWriter("file.txt");

                    var.WriteLine("Name: " + Methods.ldap_get_value<string>(user.Properties["displayName"]));
                    var.WriteLine("Domain: " + Methods.ldap_get_domainname(user));
                    var.WriteLine("Login: " + Methods.ldap_get_domainname(user) + "\\" + Methods.ldap_get_value<string>(user.Properties["samAccountName"]));
                    var.Close();
                }
            }
        }
    }
}
