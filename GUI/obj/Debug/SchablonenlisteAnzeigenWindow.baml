   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	   '    !GUI.SchablonenlisteAnzeigenWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	   -   5          � Title$#  SchablonenlisteAnzeigenWindow��5   	   $	��650��6/   $	��830��6<    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�6!    � 	Resources 5         DefaultCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value  NoRightBorderCellStyle  CenteredTextBlockStyle  CenteredColumnHeaderStyle=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�  &	        &	 �     &	     &	 )    �� 5   
   "  6)   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0.5,0.5q�60   �� 5       � ��$ Center=�6;   �� 5       � ��$ Center=�69   �� 5       � ��$ 0q�6(   5      �� 5   
   "  6/   .�5      ��  � ��
 ���� 6,   �� 5       � ��$ 0q�60   �� 5       � ��
 �����6+   5      �� 5   
   " ��6/   .�5      ��  � ��$ Center=�64   �� 5        � ��$ Center=�62   �� 5!       �@��TextAlignment$ Center=�6.   5"      �� 5%   
   "  62   .�5&      ��  � ��$ Center=�6;   5'      5(      .��5*      �  � LayoutTransform 5,   
   �� 5-      -   5        	gridScale�� �� ScaleX$ 1Q�5-   0   	 �� ScaleY$	 1Q�6;   .+�53   
   �� 
 �� Orientation$
 
Horizontal=�6   $	��Top=�6.   $
��Left=�6F   $��	10,20,0,0q�6a   $��48��6t   .+�54      �� -   5        $��SearchTextBox��54      $	��200��6+   $��5,5,5,20q�67   �� 55      -   5        $��Suchen��55      $��75��6&   $��5,5,5,20q�61   6C   56      &   System.Windows.Controls.DataGrid  58   
   -   5        $��SchablonenlisteDataGrid��58         AutoGenerateColumns .  $��	0,100,0,0q�6N      AlternatingRowBackground
 ����� �� AlternationCount$ 2��5:         GridLinesVisibility$
 None=�5;      
�������   Columns 5=      0  *System.Windows.Controls.DataGridTextColumn  5?      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Lagerplatz��6%      	CellStyle# [  5@   %   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 5A   %      Binding 5?   9   ��  �� Path$ 
Lagerplatz�5A   %     5B      $	 Typ��6%   # [  5C   %   # [ 5D   %    5B   2   �� $	 Typ�5D   %     5E      $ Schablonennummer��6%   # [  5F   %   # [ 5G   %    5E   ?   �� $ Schablonennummer�5G   %     5H      $ 
TOP/BOTTOM��6%   # [  5I   %   # [ 5J   %    5H   9   �� $ 	TOPBOTTOM�5J   %     5K      $ Nutzengröße��6%   # [  5L   %   # [ 5M   %    5K   :   �� $ Nutzengröße�5M   %     5N      $ CK��6%   # [  5O   %   # [ 5P   %    5N   1   �� $ CK�5P   %     5Q      $ 	maxZyklen��6%   # [  5R   %   # [ 5S   %    5Q   8   �� $ 	maxZyklen�5S   %     5T      $ 	aktZyklen��6%   # [  5U   %   # [ 5V   %    5T   8   �� $ 	aktZyklen�5V   %     5W      $ WarnungZyklen��6%   # [  5X   %   # [ 5Y   %    5W   <   �� $ WarnungZyklen�5Y   %     5Z      $ ausgelagert��6%   # [  5[   %   # [ 5\   %    5Z   :   �� $ ausgelagert�5\   %   4  .System.Windows.Controls.DataGridTemplateColumn  5^         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)   # [ 63   5`      �� 5c   
   -   5        $��X��5c      $��50��6   $��50��6(   $��Right=�64   $	��Top=�6P    J� RenderTransformOrigin$ 0.5,0.5&�6�   5d      5e      