   M S B A M L   `   `   `  ����   YoutubeDownloadHelper/    )YoutubeDownloadHelper.Gui.UrlManipulation   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     X TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     �Name$  	urlWindow�����	#FFF0F0F0���	#FF000000 � Title$  �� � Icon$C =/YoutubeDownloadHelper;component/Icons and Images/YDHIcon.ico��$	��553��$	��169�� � 
ResizeMode$ NoResize=�$	��169��$	��553��.��� -    �Name$ urlManipulationGrid��.+��� -    ��Name$ urlTabControl��$��0��$��0��$��Stretch=�$��Stretch=�$��0,0,-0.600000000000023,0q�.@��� -    ��Name$ 	basicView��$��
Basic View��.��� -   $ bvGrid��.+��� -    ��Name$ basicUrlText��$��12(�$��0��$��0��$	��Top=�$��22��$	��488��$��Right=�$��0,8,7.99999999999989,0q����� *	BasicText+ �� Mode$ OneWay=����� *BasicUrlBoldness+$ OneWay=����� *BasicUrlFontStyle+$ OneWay=�	 �� TextAlignment	 �� *BasicUrlTextAlignment+$ OneWay=��� -   
 ��Name$
 urlTextBlock��$
��Url:��$��0��$��0��$
��Left=�$	��Top=�$��8,7,0,0q�$��29��$��20���� -   $
 urlTextBlock_Copy��$��Format:�� �� TextAlignment$ Justify=�$��0��$��0��$
��Left=�$	��Top=�$��8,33,0,0q�$��55��$��22���� -	    ��Name$ formatComboBox�� �� Text$  ��$��11(�$��0��$��0��$��Right=�$	��Top=�$��0,34,380.8,0q�$��81��$��22������ *
FormatList+$ OneWay=� �� SelectedIndex �� *SelectedFormat+$ TwoWay=��� -
    ��Name$ basicManipulateUrlButton��$��0��$��0��$��Stretch=�$	��Top=�$��8,65,7.99999999999989,0q�$��28��$�� ���� -   $
 urlTextBlock_Copy1��$ Justify=�$��Quality:��$��58��$��22��$��0��$��0��$
��Left=�$	��Top=�$��
166,33,0,0q�%  Xceed.Wpf.Toolkit.IntegerUpDown  -     Name$ basicResolution���  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Maximum$	 600X�   MouseWheelActiveTrigger$ Focused=�$��14(�$��0��$��0��$	��Top=�$��22��$
��49.6��$
��Left=�$��
232,34,0,0q�   Minimum$ 24X��  �Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$	 120X�   Value �� *SelectedResolution+$ TwoWay=��� -   $
 basicUserInfoText�����	#FF000000{�.  $��Medium"�$��Times New Roman)�$��Oblique$�$��Normal&� �� IsHyphenationEnabled .  $ Center=�$�� ��$��0��$��0��$��Right=�$	��Top=�$��0,34,6.79999999999995,0q�$��150.00000000000006��$��22����Z� �� $
��Left=�$��Center=�$��Is Audio��$��0��$��0��$
��Left=�$	��Top=�$��
289,35,0,0q�$��79��$��21�� d� 	IsChecked �� *AudioOnlyEnabled+$ TwoWay=��� -   $ advancedView��$��Advanced View��.��� -   $ avGrid��.+��� -   $ 	avTextBox��$��Stretch=�$��Stretch=�$.��(0,0,-0.200000000000045,0.400000000000006q�$��3��$��0�� �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�$ �� HorizontalScrollBarVisibility$
 Auto=� �� TextWrapping$ NoWrap=�