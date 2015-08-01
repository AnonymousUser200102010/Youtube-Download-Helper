using System;
using System.IO;

namespace YoutubeDownloadHelper.Code
{

	public class Validation : IValidation
    {
        public void CheckOrCreateFolder (string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
        }
    }
}


