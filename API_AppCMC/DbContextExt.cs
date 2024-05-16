using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace API_AppCMC
{

	public partial class Model_CMCBacNinhEntities : DbContext, IDisposable
	{
		public bool FlagUseLog { get; set; } = false;

		public Model_CMCBacNinhEntities(string connectionString)
			: base(connectionString)
		{
			Configuration.LazyLoadingEnabled = true;
			Configuration.ProxyCreationEnabled = true;
			Configuration.EnsureTransactionsForFunctionsAndCommands = true;
			Configuration.AutoDetectChangesEnabled = false;


		}
		public override int SaveChanges()
		{
			int errCode = -1;

			if (FlagUseLog) Database.Log = s => { };


			ChangeTracker.DetectChanges();



			try
			{
				errCode = base.SaveChanges();
			}
			catch (Exception ex)
			{
				string _inner = ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : "";

				string messError = string.Format("Error occurred : \n{0}\n\n{1}",
					ex.Message, _inner);

				//MessageBox.Show(messError, Properties.Resources.MSG_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);

				messError = "";

				var valids = GetValidationErrors().ToList();

				if (valids.Count() > 0)
				{
					foreach (var _valid in valids)
					{
						foreach (var _v in _valid.ValidationErrors)
						{
							messError = messError + _v.ErrorMessage + Environment.NewLine;
						}
					}

					//MessageBox.Show(messError, Properties.Resources.MSG_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

			}



			return errCode;
		}


	}

	public static class DbContextExt
	{
		public static void AddToDatabaseItemList<T>(this DbContext context, List<T> ListEntity) where T : class
		{
			foreach (var x in ListEntity)
			{
				if (context.Entry(x).State == EntityState.Detached)
				{
					context.Entry(x).State = EntityState.Added;
				}
			}
		}


		public static bool CheckConnection(this DbContext context)
		{
			try
			{
				context.Database.Connection.Open();
				return true;
			}
			catch
			{
				return false;
			}


		}

		//Caution : Do this will reload ALL DATABASE ==> Machine Hungup if Large Data ???
		public static void RefreshEntryDatabase(this DbContext context)
		{
			foreach (var entity in context.ChangeTracker.Entries())
			{
				entity.Reload();
			}
		}

		public static bool IsDataHasChange(this DbContext context)
		{

			return context.ChangeTracker.HasChanges();

		}

		public static void RefreshEntries<T>(this DbContext context) where T : class
		{
			foreach (var entity in context.ChangeTracker.Entries<T>())
			{
				entity.Reload();
			}
		}

		public static void RollbackChange<T>(this DbContext context) where T : class
		{
			var changedEntries = context.ChangeTracker.Entries<T>()
				.Where(x => x.State != EntityState.Unchanged).ToList();

			foreach (var entry in changedEntries)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						entry.CurrentValues.SetValues(entry.OriginalValues);
						entry.State = EntityState.Unchanged;
						break;
					case EntityState.Added:
						entry.State = EntityState.Detached;
						break;
					case EntityState.Deleted:
						entry.State = EntityState.Unchanged;
						break;
				}
			}
		}

		public static void RollbackChange(this DbContext context)
		{
			var changedEntries = context.ChangeTracker.Entries()
				.Where(x => x.State != EntityState.Unchanged).ToList();

			foreach (var entry in changedEntries)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						entry.CurrentValues.SetValues(entry.OriginalValues);
						entry.State = EntityState.Unchanged;
						break;
					case EntityState.Added:
						entry.State = EntityState.Detached;
						break;
					case EntityState.Deleted:
						entry.State = EntityState.Unchanged;
						break;
				}
			}
		}

		public static void ReloadNavigationProperty<TEntity, TElement>(
			this DbContext context,
			TEntity entity,
			Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
			where TEntity : class
			where TElement : class
		{
			context.Entry(entity).Collection<TElement>(navigationProperty).Query();
		}

	}

	public static class ConnectionTools
	{
		private const string providerName = "System.Data.SqlClient";
		private const string DATAMODELNAME = "Model_CMC"; // MUST SET WITH EDMX file name

		public static SqlConnectionStringBuilder ConnectionStringBuilder(string _DataSource, string _InitialCatalog, string _UserID, string _Password)
		{

			return new SqlConnectionStringBuilder()
			{
				DataSource = _DataSource,           //"163.44.192.123",
				InitialCatalog = _InitialCatalog,   // "HRMDB",
				UserID = _UserID,                   // "tkv5user",
				Password = _Password,               // "123456a$",
				PersistSecurityInfo = true,
				IntegratedSecurity = false,
				ConnectRetryCount = 5,
				ConnectRetryInterval = 2,
				ConnectTimeout = 10,
				Encrypt=true,
				TrustServerCertificate=true

			};
		}
		public static string BuildConnectionStringDirect(string _DataSource, string _InitialCatalog, string _UserID, string _Password)
		{
			SqlConnectionStringBuilder builder = ConnectionStringBuilder(_DataSource, _InitialCatalog, _UserID, _Password);

			EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

			string DataModelName = DATAMODELNAME; //"HRMDB";

			string providerString = builder.ConnectionString;

			//Set the provider name.
			entityBuilder.Provider = providerName;

			// Set the provider-specific connection string.
			entityBuilder.ProviderConnectionString = providerString;

			// Set the Metadata location.
			entityBuilder.Metadata = string.Format(@"res://*/{0}.csdl|res://*/{1}.ssdl|res://*/{2}.msl", DataModelName, DataModelName, DataModelName);

			return entityBuilder.ToString();

		}
		

        //Use for WebAPI
        public static string BuildConnectionString()
        {
            return BuildConnectionStringDirect(
               _DataSource: "dbdev.namanphu.vn",
               _InitialCatalog: "Model_CMCBacNinh",
               _UserID: "notification_user",
               _Password: "123456a$");
        }
        public static string BuildConnectionStringServerTest()
		{
			//return BuildConnectionStringDirect(
			//   _DataSource: "163.44.192.123",
			//   _InitialCatalog: "logistic_db",
			//   _UserID: "logistic_db",
			//   _Password: "123456a$");
			string sName = "d" + "b" + "." + "n" + "a" + "m" + "a" + "n" + "p" + "h" + "u" + "." + "v" + "n";
			string dName = "c" + "u" + "s" + "t" + "o" + "m" + "e" + "r" + "d" + "b";
			string uName = "c" + "u" + "s" + "t" + "o" + "m" + "e" + "r" + "u" + "s" + "e" +"r";
			string Pw = "123456a$";
			return Security.EncryptString($"Server = {sName}; Database = {dName}; User Id = {uName}; Password = {Pw};",Security.KeyGen("Locy2"));
		}

	}
	public static class Security
	{
		internal static string EncryptString(string PlainText, string[] KeyCouple, bool pCompress = true)
		{
			string rt;
			using (RijndaelManaged rijAlg = new RijndaelManaged())
			{
				rijAlg.BlockSize = 192;
				rijAlg.Key = Encoding.UTF8.GetBytes(KeyCouple[0]);
				rijAlg.IV = Encoding.UTF8.GetBytes(KeyCouple[1]);
				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
					if ((pCompress))
					{
						using (System.IO.Compression.DeflateStream myCompress = new System.IO.Compression.DeflateStream(csEncrypt, System.IO.Compression.CompressionMode.Compress))
						{
							using (StreamWriter swEncrypt = new StreamWriter(myCompress))
							{
								// Write all data to the stream.
								swEncrypt.Write(PlainText);

							}
						}
					}
					else
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							// Write all data to the stream.
							swEncrypt.Write(PlainText);
						}
					rt = ByteArrayToString(msEncrypt.ToArray());
				}
			}
			return rt;
		}

		public static string DecryptString(string pEncryptedString, string[] KeyCouple, bool pCompress = true)
		{
			// Dim rt As String
			byte[] cipherText;
			string plaintext;

			using (RijndaelManaged rijAlg = new RijndaelManaged())
			{
				rijAlg.BlockSize = 192;
				rijAlg.Key = Encoding.UTF8.GetBytes(KeyCouple[0]);
				rijAlg.IV = Encoding.UTF8.GetBytes(KeyCouple[1]);

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
				cipherText = StringToByteArray(pEncryptedString);

				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						if (pCompress == true)
						{
							using (System.IO.Compression.DeflateStream myDecompress = new System.IO.Compression.DeflateStream(csDecrypt, System.IO.Compression.CompressionMode.Decompress))
							{
								using (StreamReader srDecrypt = new StreamReader(myDecompress))
								{
									plaintext = srDecrypt.ReadToEnd();
								}
							}
						}
						else
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{
								plaintext = srDecrypt.ReadToEnd();
							}
					}
				}
			}
			return plaintext;
		}

		private static string GetMd5Hash(string input)
		{
			using (MD5 md5Hash = MD5.Create())
			{
				byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
				return Convert.ToBase64String(data);
			}
		} // GetMd5Hash

		private static byte[] StringToByteArray(string pString, string mode = "hex")
		{
			byte[] rt = null;
			if (mode == "base64")
			{
				rt = Convert.FromBase64String(pString);
			}
			else if (mode == "hex")
			{
				int NumberChars = pString.Length;
				byte[] bytes = new byte[NumberChars / 2];
				for (int i = 0; i < NumberChars; i += 2)
					bytes[i / 2] = Convert.ToByte(pString.Substring(i, 2), 16);
				rt = bytes;
			}
			return rt;
		}
		public static string[] KeyGen(string firstKey)
		{
			string[] rt = new string[2];
			rt[0] = GetMd5Hash(firstKey);
			rt[1] = GetMd5Hash(@"{Dao@Ngoc#Gia*Han}");
			return rt;
		}
		private static string ByteArrayToString(byte[] pArray, string mode = "hex")
		{
			string rt = "";
			if (mode == "base64")
				rt = Convert.ToBase64String(pArray);
			else if (mode == "hex")
			{
				StringBuilder hex = new StringBuilder(pArray.Length * 2);
				foreach (byte b in pArray)
					hex.AppendFormat("{0:x2}", b);
				rt = hex.ToString();
			}
			return rt;
		}
	}
}