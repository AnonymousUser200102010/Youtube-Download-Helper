﻿using System;
using System.IO;

namespace YoutubeDownloadHelper
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


