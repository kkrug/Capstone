   M S B A M L   `   `   `  ����  BodyIndexBasics-WPF9    3Microsoft.Samples.Kinect.BodyIndexBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Kinect Physical Therapy��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�5   	   ��5   	   �� *&$SystemParameters.PrimaryScreenHeight+��6A   �� *%#SystemParameters.PrimaryScreenWidth+.��5	      � -   5         �Name$ MainWindowGrid��5	      $��Stretch=�6#   $��Stretch=�6A   .+�5
   
   �� -   5         ��Name$ kinectVideoImage��5
      $
��1920��6*   $
��1080��67   �� 5   
   -   5         ��Name$ started��5      $	��155��6!   $��50��6-   $��Exercise Started��69   $��20(�6T   $��Center=�6b   ���#484848���#66FF99$��-229,-78,835.333,533.333q�6�    K� ZIndex$	 100��6�   $y�Hidden=�6�   �� 5   
   -   5        $ replabel��5      $	��155��6"   $��50��6.   $��Rep Number:��6:   $��24(�6P   $��Center=�6^   ���#484848���#66FF99$��-229,-28,835.333,483.333q�6�   $ 10��6�   �� 5   
   -   5        $ 
scorelabel��5      $	��155��6$   $��50��60   $��Score:��6<   $��24(�6M   $��Center=�6[   ���#484848���#66FF99$��-229,62,835.333,393.333q�6�   $ 10��6�   �� 5   
   -   5        $ MidRep��5      $�� ��6    $	��155��6+   $��75��67   $��42(�6C   $��Center=�6Q   ���#484848���#66FF99$��-229,4,835.333,426.333q�6�   $ 1��6�   �� 5   
   -   5        $��Feedback��5      $�� ��6    $	��155��6+   $��75��67   $��42(�6C   $��Center=�6Q   ���#484848���#66FF99$��-229,88,835.333,342.333q�6�   $ 1��6�   �� 5   
   -	   5         ��Name$ 
bodyCanvas��5      $
��1920��6%   $
��1080��62   $
��Left=�6@   $	��Top=�6[   �� 5   
   -
   5        $ 
pathCanvas��5      $
��1920��6%   $
��1080��62   $
��Left=�6@   $	��Top=�6[   5      5      