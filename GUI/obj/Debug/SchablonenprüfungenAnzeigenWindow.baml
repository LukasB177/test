   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	   ,    &GUI.SchablonenprüfungenAnzeigenWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	   -   5          � Title$(  "SchablonenprüfungenAnzeigenWindow��5   	   $	��450��63   $	��800��6@    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�6!    � 	Resources 5         DefaultCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value  NoRightBorderCellStyle��%   &	        &	 y     �� 5   
   "  6)   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0.5,0.5q�60   5      �� 5   
   "  6/   .�5      ��  � ��
 ���� 6,   �� 5       � ��$ 0q�60   �� 5       � ��
 �����6+   5      5      .��5      �  � LayoutTransform 5   
   �� 5      -   5        windowScale�� �� ScaleX$ 1Q�5   2   	 �� ScaleY$	 1Q�6=   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        
  Name$
 VorgangeDataGrid��5         AutoGenerateColumns .  $��Stretch=�6I   $��Stretch=�6g      AlternatingRowBackground
 ����� �� AlternationCount$ 2��5         GridLinesVisibility$
 None=�6(   ��5   �   �� *
Vorgange+   Columns 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ Datum��6%      	CellStyle# [  6p   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Datum+ �� StringFormat$ yyyy-MM-dd HH:mm:ss��6p     5"      $ 	CK-Nummer��6%   # [  6V    68   �� *	CK-Nummer+6V     5#      $ 
Bearbeiter��6%   # [  6X    69   �� *
Bearbeiter+6X   4  .System.Windows.Controls.DataGridTemplateColumn  5$         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)   # [ 63   5&      �� 5)   
   -   5        $��X��5)      $��50��6   $��50��6(   $��Right=�64   $	��Top=�6P    J� RenderTransformOrigin$ 0.5,0.5&�6�   5+      5,      