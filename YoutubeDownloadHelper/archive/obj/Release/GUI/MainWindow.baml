   M S B A M L   `   `   `  ����   YoutubeDownloadHelper*    $YoutubeDownloadHelper.Gui.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     X TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     �Name$  
mainWindow�����	#FFF0F0F0���	#FFF0F0F0$��0,0,0,0q� � Title$ Youtube Download Helper�� � 
ResizeMode$ CanMinimize=�$	��688��$	��534�� � Icon$o iC:\Users\Erik\Documents\GitHub\Youtube-Download-Helper\YoutubeDownloadHelper\Icons and Images\YDHIcon.ico��{��� *WindowEnabled+.��� $��Stretch=�$��0,0,0,0q�$��Stretch=�.+��� -    ��Name$ mainTabControl�����	#FFF0F0F0���	#FF000000$��0.5,0.5,0.5,0.5q�$��0��$��0��$��Stretch=�$��Stretch=�$"��0,22,-0.600000000000023,68.2q�.@��� -    ��Name$ queueTab��$��Queue�����	#FF000000.��� -    �Name$ 	queueGrid��$	��NaN��$	��NaN��.+��� -    ��Name$ addUrlButton��$��1,1,1,1q�$��0��$��0��$
��30.4��$��-0.799999999999898,0,0,0q�$
��Left=�$	��Top=�$	��338��$��
Add Url(s)���� -   $ modifyUrlButton��$��Modify Url(s)��$��	338,0,0,0q�$��0��$��0��$��30��$	��334��$��Stretch=�$	��Top=�$��0,0,0,0q�{�.  �� -   $ moveQueuedItemUp��{�.  $��0��$��0��$
��Left=�$	��Top=�$��	-1,30,0,0q�$��25.800000000000068��$	��174����Z� .��� -   $ upArrowGrid��$	��173��$��19��.+��� -	    ��Name$ upArrowImage��	 J� RenderTransformOrigin$	 0.5,0.5&�$
��Fill=�
 �� StretchDirection$

 Both=�$��0��$��0��$��Center=�$��Center=�$��0,0,0,0q�$��16��$	��164��$t��nC:\Users\Erik\Documents\GitHub\Youtube-Download-Helper\YoutubeDownloadHelper\Icons and Images\UpArrowImage.png���� -
   $ moveQueuedItemDown��{�.  $��0��$��0��$
��Left=�$��Bottom=�$��-1,0,0,-0.199999999999989q�$��25.800000000000068��$	��172����Z� .��� -   $ downArrowGrid��.+��� -   $ downArrowImage��$	 0.5,0.5&�$
��Fill=�$

 Both=�$��0,0,0,0q�$��Center=�$��Center=�$��0��$��0��$	��164��$��16��$v��pC:\Users\Erik\Documents\GitHub\Youtube-Download-Helper\YoutubeDownloadHelper\Icons and Images\DownArrowImage.png���� -    ��Name$ queueListView�� �� SelectionMode$ Single=�$
��Left=�$��0��$��0��$��Stretch=�$��Stretch=�$2��,24.8,30,-1.60000000000002,-0.199999999999989q� �� SelectedIndex �� *CurrentlySelectedQueueIndex+ �� Mode$ TwoWay=����� *Videos+ �� View �� .P���  �� Width �� *QueuePositionTagWidth+$ TwoWay=� �� DisplayMemberBinding �� *
Position+���� 
 � Tag$ NumberInQueue��$��
# in Queue��$	��NaN����  �� *QueueLocationTagWidth+$ TwoWay=� �� *
Location+���� $ UrlLocation��$	��Url��$	��NaN����  �� *QueueQualityTagWidth+$ TwoWay=� �� *	Quality+���� $ 
UrlQuality��$��Quality��$	��NaN����  �� *QueueFormatTagWidth+$ TwoWay=� �� *Format+���� $ 	UrlFormat��$��Format��$	��NaN����  �� *QueueIsAudioTagWidth+$ TwoWay=� �� *IsAudioFile+���� $ isAudioFile��$��Audio Track Only��$	��NaN��o� -    o�Name$ mainMenu��$��0��$��0��$��Stretch=�$	��Top=�$��0,3,1.39999999999998,0q�$��19��.@�m� -    m�Name$ optionsMenu��$��Options����. $	 0.5,0.5&�m� -   $ helpMenu��$
��Help��.@�m� -   $ aboutMenuItem��$��About��� $��0��$��0��$
��Left=�$��Bottom=�$��0,0,0,0.199999999999989q�$��681.6��$��35��.+�� -    �Name$ downloadProgressBar�� w� Text$  ��$	 0.4109,0.1591&�$��0��$��0��$
��Left=�$��Bottom=�$��
0,0,0,-1.8q�$��678.4��$��35�� � Value �� $ OneWay=� �� Path$ CurrentDownloadProgress�����  �� 
StartPoint$	 0,0&� �� EndPoint$	 0,1&����  � Color$ 	#FF000000�� � Offset$ 0Q�� $ 	#FF8C8C8C��$ 1Q����� $	 0,0&�$	 1,0&���� $ 	#FFFFFFFF��$ 0Q�� $ 	#FFC7C7C7��$	 0.5Q�� $ 	#FFBABABA��$ 1Q��� -    ��Name$ downloadProgressText��$��Center=�$��Center=�$
��Thin"�$��12(����� $ OneWay=�$ CurrentDownloadOutputText� � ColumnDefinitions �� -   $ startDownloadingButton��$��Start Downloading��$��0��$��0��$
��Left=�$��Bottom=�$��
0,0,0,33.2q�$��678.4��$��30��{�.   