using System;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    public static class GlobalVariables
    {
		
        public static ObservableCollection<Tuple<string, int, VideoType>> urlList = new ObservableCollection<Tuple<string, int, VideoType>> ();
		
    }
}


