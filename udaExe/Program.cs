using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;

namespace udaExe {
	class Program {
		static void Main(string[] args) {
			string ldapEntry;
			string fileName;
			int cantUsuarios = 0;
			if (!GetParameters(args, out ldapEntry, out fileName)) {
				Console.Out.WriteLine("uso: udaExe ldap fichero");
				Console.Out.WriteLine("Ejemplo: udaExe \"LDAP://OU=Users,dc=microsoft,dc=local\" \"c:\\usuarios.txt\"");

			} else {
				Console.Out.WriteLine("Buscando usuarios:");

				using (StreamWriter sw = File.CreateText(fileName)) {
					try {
						DirectoryEntry directoryObject = new DirectoryEntry(ldapEntry);
						foreach (DirectoryEntry child in directoryObject.Children) {
							string displayName = GetProperty(child, "displayName");
							string distinguishedName = GetProperty(child, "distinguishedName");
							string samAccountName = GetProperty(child, "samAccountName");
							child.Close();
							child.Dispose();
							sw.WriteLine($"{displayName}#{samAccountName}#{distinguishedName}");
							cantUsuarios++;
						}
						directoryObject.Close();
						directoryObject.Dispose();
					} catch (DirectoryServicesCOMException ex) {
						Console.Error.WriteLine("Error: " + ex.Message.ToString());
					}
				}
				Console.Out.WriteLine(cantUsuarios.ToString() + " usuarios encontrados, fichero generado: " + fileName);
			}
		}

		public static string GetProperty(DirectoryEntry de, string PropertyName) {
			if (de.Properties.Contains(PropertyName)) {
				return de.Properties[PropertyName][0].ToString();
			} else {
				return string.Empty;
			}
		}

		public static bool GetParameters(string[] args, out string ldap, out string fileName) {
			ldap = String.Empty;
			fileName = @"c:\users.txt";
			if (args.Length < 1) {
				return false;
			}
			ldap = args[0];
			if (args.Length >= 2) {
				fileName = args[1];
			}
			return true;
		}
	}
}
