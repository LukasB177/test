   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	   !    GUI.WarnungenAnzeigenWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	   -   5          � Title$  WarnungenAnzeigenWindow��5   	   $	��450��6)   $	��800��66    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�6!    � 	Resources 5         DefaultCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value  MeldungstextCellStyle �� Triggers �� Binding	 �� Value  NoRightBorderCellStyle��%H  &	        &	 y     &	 �    �� 5   
   "  6)   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0.5,0.5q�60   5      �� 5   
   "  6.   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0,0.5q�60   �� 5       � ��
 �����6+    5      �� 5       6   �� *Meldungstext+%	 !WARNUNG: Standzeit bald erreicht.6?   .a�5      ��  � ��
 � ���63   5      �� 5       6   �� *Meldungstext+%	 !WARNUNG: Standzeit bald erreicht!6?   .a�5      ��  � ��
 � ���63   5      �� 5       6   �� *Meldungstext+!	 KRITISCH: Standzeit erreicht!6?   .a�5      ��  � ��
 �  ��63   5      5!      �� 5"   
   "  6/   .�5#      ��  � ��
 ���� 6,   �� 5$       � ��$ 0q�60   5%      5&      .��5*      � 
 � LayoutTransform
 5+   
   �� 5,      -   5        windowScale�� �� ScaleX$ 1Q�5,   2    �� ScaleY$ 1Q�6=   .+�5/   
   &   System.Windows.Controls.DataGrid  -   5          Name$ VorgangeDataGrid��5/         AutoGenerateColumns .  $��Stretch=�6I   $��Stretch=�6g   
�������   RowBackground
 �����   GridLinesVisibility$
 None=�50   <      Columns 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn   Header$ Datum��6%      	CellStyle# [  6N   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *datum+6N     53      $ 
Lagerplatz��6%   # [  6X    69   �� *
Lagerplatz+6X     54      $	 Typ��6%   # [  6J    62   �� *Typ+6J     55      $ Schablonennummer��6%   # [  6d    6?   �� *Schablonennummer+6d     56      $ TOP/BOT��6%   # [  6Q    66   �� *TOPBOT+6Q     57      $	 CK#��6%   # [  6J    62   �� *CK#+6J     58      $ Benutzer��6%   # [  6T    67   �� *
Benutzer+6T   4  .System.Windows.Controls.DataGridTemplateColumn  59      $ Meldungstext��6)   # [ 6?      CellTemplate 5:      �� 5;      .R�5<      �� ��6(   �� *Meldungstext+��5=   "   �� 5>   &   " ��6,    5?   *   �� 5@   .    6:   �� *Meldungstext+%	 !WARNUNG: Standzeit bald erreicht.6[   .a�5A   2   ��  � ��
 � ���6O   5B   /   �� 5C   .    6:   �� *Meldungstext+%	 !WARNUNG: Standzeit bald erreicht!6[   .a�5D   2   ��  � ��
 � ���6O   5E   /   �� 5F   .    6:   �� *Meldungstext+!	 KRITISCH: Standzeit erreicht!6[   .a�5G   2   ��  � ��
 �  ��6O   5H   /   5J   '   5L      5M      5O        5P         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)   # [ 63   5R      �� 5U   
   -   5        $��X��5U      $��50��6   $��50��6(   $��Right=�64   $	��Top=�6P    J� RenderTransformOrigin$ 0.5,0.5&�6�   5W      5X      