   M S B A M L   `   `   `  ����D  @QuickDiff, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ViewModels
ViewModels  5   	   clr-namespace:ViewsViews  5   	       QuickDiff.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   'vmclr-namespace:ViewModels     5   	   %viewsclr-namespace:Views     5   	     � Title$  	QuickDiff��5   	   $	��350��6   $	��525��6(   .��5      � 
������  � DataContext 5   
   $   ViewModels.MainWindowViewModel  5	       � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��5 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��3��6    �� Orientation$ 
Horizontal=�6)   
������ .+�5      ��    GlassButtonSquare#���   6   $��2q�6M   $��	12,2,12,2q�6X   $	��<<-��6�   ��6l   �� *BackFolders+6�   �� 5      #���   6   $��2q�6M   $��	12,2,12,2q�6X   $��Refresh��6�   ��6l   �� *RefreshFolders+6�   5         Views.FileItemsView  5   
   $��1��6    6+   ��  �� Path$ FileItemsLeft�  5   
   $��1��6   $��2��6+    6;   �� $ FileItemsRight��� 5   
   $��1��6   $��1��6$   $��Stretch=�64   $��5��6R     foregroundBrush#���  6\   5      5      