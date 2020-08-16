using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class Photo
    {
        string m_Text;
        string m_PhotoId;
        string m_MainPhotoLink;
        string m_MinPhotoLink;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public string PhotoId
        {
            get { return m_PhotoId; }
        }

        public string MainPhotoLink
        {
            get { return m_MainPhotoLink; }
        }

        public string MinPhotoLink
        {
            get { return m_MinPhotoLink; }
        }

        public Photo(string PhotoId, string Text, string MainPhotoLink, string MinPhotoLink)
        {
            m_PhotoId = PhotoId;
            m_Text = Text;
            m_MainPhotoLink = MainPhotoLink;
            m_MinPhotoLink = MinPhotoLink;
        }
    }
}
