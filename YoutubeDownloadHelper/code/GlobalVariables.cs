using System;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExtractor;

namespace YoutubeDownloadHelper
{

    public static class GlobalVariables
    {
		
        public static Collection<Tuple<string, int, VideoType>> urlList = new Collection<Tuple<string, int, VideoType>> ();
		
    }
}


