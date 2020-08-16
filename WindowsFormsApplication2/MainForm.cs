using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Web;
using xNet;

namespace WindowsFormsApplication2
{
    public partial class MainForm : Form
    {
        public const string m_cApplicationKey = "CBANFINLEBABABABA";
        public const string CLIENT_ID = "1255627776";
        public const string CLIENT_SECRET = "CA5EBF87130C9352491FBA6A";
        public string m_AccessToken = "";
        public const string apiURL = "https://api.ok.ru/fb.do";
        public List<Photo> m_PhotoList = new List<Photo>();
        public static bool m_bIsOnline = true;

        #region Шифровалки
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();
            lvPosts.Columns.Add("ID фотографии");
            lvPosts.Columns.Add("Описание");
            lvPosts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvPosts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #region Методы, работающие с API
        void UpdateList()
        {
            lvPosts.Items.Clear();
            foreach (Photo photo in m_PhotoList)
            {
                ListViewItem NewItem = new ListViewItem(new[]
                {
                    photo.PhotoId,
                    photo.Text
                })
                {
                    Tag = photo
                };
                lvPosts.Items.Add(NewItem);
            }
        }

        void GetUserInfo()
        {
            try
            {
                string sig;
                using (MD5 md5Hash = MD5.Create())
                {
                    string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                    sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "method=users.getCurrentUser" + secret_sig);
                };
                HttpRequest request = new HttpRequest();
                request.AddUrlParam("method", "users.getCurrentUser");
                request.AddUrlParam("application_key", m_cApplicationKey);
                request.AddUrlParam("sig", sig);
                request.AddUrlParam("access_token", m_AccessToken);
                string result = request.Get(apiURL).ToString();
                JObject jObj = JObject.Parse(result);
                string UserName = jObj["name"].ToString();
                string AvatarUrl = jObj["pic_3"].ToString();
                lblName.Text = UserName;
                pbAvatar.ImageLocation = AvatarUrl;
                if (pbAvatar.ImageLocation == null)
                {
                    pbAvatar.ImageLocation = "NoPhoto.jpg";
                }
                pbAvatar.Load();
            }
            catch
            {
                MessageBox.Show("Проверьте интернет-соединение!");
            }
        }

        Photo NewPhoto(string CurPhoto)
        {
            string[] parse = CurPhoto.Split('\r');
            string PhotoId = (parse[2].Split(':'))[1];
            PhotoId = PhotoId.Substring(2, PhotoId.Length - 4);
            string MinPhotoLink = (parse[4].Split(':'))[2];
            MinPhotoLink = "https://" + MinPhotoLink.Substring(2, MinPhotoLink.Length - 4);
            string MainPhotoLink = (parse[5].Split(':'))[2];
            MainPhotoLink = "https://" + MainPhotoLink.Substring(2, MainPhotoLink.Length - 4);
            string PhotoText = (parse[6].Split(':'))[1];
            PhotoText = PhotoText.Substring(2, PhotoText.Length - 4);

            return new Photo(PhotoId, PhotoText, MainPhotoLink, MinPhotoLink);
        }

        List<Photo> GetUserPhotos()
        {
            List<Photo> UserPhotos = new List<Photo>();
            try
            {
                string sig;
                using (MD5 md5Hash = MD5.Create())
                {
                    string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                    sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "method=photos.getPhotos" + secret_sig);
                };
                HttpRequest request = new HttpRequest();
                request.AddUrlParam("method", "photos.getPhotos");
                request.AddUrlParam("application_key", m_cApplicationKey);
                request.AddUrlParam("sig", sig);
                request.AddUrlParam("access_token", m_AccessToken);
                string result = request.Get(apiURL).ToString();
                JObject jObj = JObject.Parse(result);
                string s = jObj["photos"].ToString();

                string[] Photos = s.Split('{');
                for (int i = 1, Count = Photos.Length; i < Count; i++)
                {
                    UserPhotos.Add(NewPhoto(Photos[i]));
                }
            }
            catch
            {
                MessageBox.Show("Проверьте интернет-соединение!");
            }

            return UserPhotos;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Photo UpdPhoto;
                Photo photo;
                try
                {
                    ListViewItem selectedItem = lvPosts.SelectedItems[0];
                    photo = (Photo)selectedItem.Tag;
                    UpdPhoto = m_PhotoList.Find(x => x.PhotoId == photo.PhotoId);
                }
                catch
                {
                    MessageBox.Show("Выберите фотографию из списка!");
                    return;
                }
                FrmUpdate UpdPost = new FrmUpdate(FrmUpdate.FormType.Update)
                {
                    Post = UpdPhoto.Text,
                    ImageUrl = UpdPhoto.MinPhotoLink
                };
                if (UpdPost.ShowDialog() == DialogResult.OK)
                {
                    string sig;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                        sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "description=" + UpdPost.Post + "method=photos.editPhoto" + "photo_id=" + photo.PhotoId + secret_sig);
                    };
                    HttpRequest request = new HttpRequest();
                    request.AddUrlParam("method", "photos.editPhoto");
                    request.AddUrlParam("application_key", m_cApplicationKey);
                    request.AddUrlParam("sig", sig);
                    request.AddUrlParam("access_token", m_AccessToken);
                    request.AddUrlParam("photo_id", photo.PhotoId);
                    request.AddUrlParam("description", UpdPost.Post);

                    string result = request.Post(apiURL).ToString();
                    if (result == "true")
                    {
                        UpdPhoto.Text = UpdPost.Post;
                        UpdateList();
                    }
                }
            }

            catch
            {
                MessageBox.Show("Проверьте интернет-соединение!");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmUpdate FrmCreate = new FrmUpdate(FrmUpdate.FormType.Insert);
            if (FrmCreate.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Получение URL
                    string sig;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                        sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "method=photosV2.getUploadUrl" + secret_sig);
                    };
                    HttpRequest request = new HttpRequest();
                    request.AddUrlParam("method", "photosV2.getUploadUrl");
                    request.AddUrlParam("application_key", m_cApplicationKey);
                    request.AddUrlParam("sig", sig);
                    request.AddUrlParam("access_token", m_AccessToken);
                    string result = request.Post(apiURL).ToString();
                    JObject jObj = JObject.Parse(result);
                    string[] PhotoId = jObj["photo_ids"].ToString().Split('"');
                    Photo NewPhoto = new Photo(PhotoId[1], FrmCreate.Post, FrmCreate.ImageUrl, jObj["upload_url"].ToString());

                    //Загрузка на сервер
                    request = new HttpRequest();
                    string[] Name = NewPhoto.MainPhotoLink.Split('\\');

                    request.AddFile(Name[Name.Length - 1], NewPhoto.MainPhotoLink);
                    result = request.Post(NewPhoto.MinPhotoLink).ToString();
                    jObj = JObject.Parse(result);
                    string[] token = jObj["photos"].ToString().Split('{');
                    token = token[2].Split('"');
                    string photoToken = System.Uri.EscapeDataString(token[3]);
                    string photoID = System.Uri.EscapeDataString(NewPhoto.PhotoId);

                    //Коммит фоточки
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                        sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "comment=" + FrmCreate.Post + "method=photosV2.commit" + "photo_id=" + NewPhoto.PhotoId + "token=" + token[3] + secret_sig);
                    };
                    request = new HttpRequest();
                    request.AddUrlParam("method", "photosV2.commit");
                    request.AddUrlParam("application_key", m_cApplicationKey);
                    request.AddUrlParam("comment", FrmCreate.Post);
                    request.AddUrlParam("photo_id", photoID);
                    request.AddUrlParam("token", photoToken);
                    request.AddUrlParam("sig", sig);
                    request.AddUrlParam("access_token", m_AccessToken);

                    result = request.Post(apiURL).ToString();
                    m_PhotoList = GetUserPhotos();
                    UpdateList();
                }
                catch
                {
                    MessageBox.Show("Проверьте интернет-соединение!");
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            m_PhotoList = GetUserPhotos();
            UpdateList();
        }
       
        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                Photo photo; 
                try
                {
                    ListViewItem selectedItem = lvPosts.SelectedItems[0];
                    photo = (Photo)selectedItem.Tag;
                }
                catch
                {
                    MessageBox.Show("Выберите фотографию из списка!");
                    return;
                }
                string sig;
                using (MD5 md5Hash = MD5.Create())
                {
                    string secret_sig = GetMd5Hash(md5Hash, m_AccessToken + CLIENT_SECRET);
                    sig = GetMd5Hash(md5Hash, "application_key=" + m_cApplicationKey + "method=photos.deletePhoto" + "photo_id=" + photo.PhotoId + secret_sig);
                };
                HttpRequest request = new HttpRequest();
                request.AddUrlParam("method", "photos.deletePhoto");
                request.AddUrlParam("application_key", m_cApplicationKey);
                request.AddUrlParam("sig", sig);
                request.AddUrlParam("access_token", m_AccessToken);
                request.AddUrlParam("photo_id", photo.PhotoId);

                string result = request.Raw(HttpMethod.DELETE, apiURL).ToString();
                if (result == "true")
                {
                    Photo DelPhoto = m_PhotoList.Find(x => x.PhotoId == photo.PhotoId);
                    m_PhotoList.Remove(DelPhoto);
                    UpdateList();
                }
            }
            catch
            {
                MessageBox.Show("Проверьте интернет соединение!");
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem selectedItem = lvPosts.SelectedItems[0];
                Photo photo = (Photo)selectedItem.Tag;
                string Url = m_PhotoList.Find(x => x.PhotoId == photo.PhotoId).MainPhotoLink;
                PhotoView photoView = new PhotoView(Url);
                photoView.Show();
            }
            catch
            {
                MessageBox.Show("Выберите фотографию из списка!");
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            SuperMainForm Warning = new SuperMainForm();
            if (Warning.ShowDialog() == DialogResult.OK)
            {
                m_AccessToken = "";
                StreamWriter Writer = new StreamWriter("AccessToken.txt");
                Writer.WriteLine(m_AccessToken);
                Writer.Close();
                m_bIsOnline = false;
                this.Close();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("AccessToken.txt"))
            {
                byte[] AccessToken, KeyByte, IVByte;
                StreamReader Reader = new StreamReader("AccessToken.txt");
                string Access = Reader.ReadLine();
                Reader.Close();
                if (!String.IsNullOrWhiteSpace(Access))
                {
                    try
                    {
                        Reader = new StreamReader("Key.txt");
                        string Key = Reader.ReadLine();
                        Reader.Close();
                        Reader = new StreamReader("IV.txt");
                        string IV = Reader.ReadLine();
                        Reader.Close();
                        AccessToken = Convert.FromBase64String(Access);
                        KeyByte = Convert.FromBase64String(Key);
                        IVByte = Convert.FromBase64String(IV);
                        m_AccessToken = DecryptStringFromBytes(AccessToken, KeyByte, IVByte);
                    }
                    catch
                    {
                        MessageBox.Show("Я не смог разгадать этот сложный шифр и упаль :(");
                    }
                }
            }
            if (String.IsNullOrWhiteSpace(m_AccessToken))
            {
                AuthForm authForm = new AuthForm(this);
                authForm.ShowDialog();
            }
            if (!String.IsNullOrWhiteSpace(m_AccessToken))
            {
                m_PhotoList = GetUserPhotos();
                GetUserInfo();
                UpdateList();
                this.Show();
            }
            else Close();

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(m_AccessToken))
            {
                StreamWriter Writer = new StreamWriter("AccessToken.txt");
                byte[] AccessToken, Key, IV;
                using (Rijndael myRijndael = Rijndael.Create())
                {
                    Key = myRijndael.Key;
                    IV = myRijndael.IV;
                    AccessToken = EncryptStringToBytes(m_AccessToken, Key, IV);

                }
                string str = Convert.ToBase64String(AccessToken);
                Writer.WriteLine(str);
                Writer.Close();
                Writer = new StreamWriter("Key.txt");
                str = Convert.ToBase64String(Key);
                Writer.WriteLine(str);
                Writer.Close();
                Writer = new StreamWriter("IV.txt");
                str = Convert.ToBase64String(IV);
                Writer.WriteLine(str);

                Writer.Close();
            }
        }
    }
}
