   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	    GUI&    GUI.lagervorgängeAnzeigenWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   -   5          � Title$"  lagervorgängeAnzeigenWindow��5   	   $	��650��6-   $	��830��6:    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�6!    � 	Resources 5         DefaultCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value  MeldungstextCellStyle �� Triggers �� Binding	 �� Value
 P� Property P� Value  NoRightBorderCellStyle  CenteredTextBlockStyle  CenteredColumnHeaderStyle=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%H  &	        &	 �     &	 �    &	      &	 �    �� 5   
   "  6)   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0.5,0.5q�60   �� 5       � ��$ Center=�6;   �� 5       � ��$ Center=�69   �� 5       � ��$ 0q�6(   5      �� 5   
   "  6.   .�5      ��  � ��
 �   �6+   �� 5       � ��$
 Left=�6;   �� 5       � ��$ Center=�69    5      �� 5       6   �� *Meldungstext+#	 VORGANG: Schablone eingelagert.6?   .a�5       ��  � ��
 � � �63   5!      �� 5"       6   �� *Meldungstext+#	 VORGANG: Schablone ausgelagert.6?   .a�5#      ��  � �� �	#FF13931363   5$      �� 5%       6   �� *Meldungstext+!	 VORGANG: Schablone archiviert6?   .a�5&      ��  � �� �	#FFACAC2463   5'      P� 5(      
 �@ 
IsSelected . . �5)      ��  � ��
 �����63   5*      5,      �� 5/   
   "  6/   .�50      ��  � ��
 ���� 6,   �� 51       � ��$ 0q�60   �� 52       � ��
 �����6+   53      �� 56   
   " ��6/   .�57      ��  � ��$ Center=�64   �� 58       � ��$ Center=�62   �� 59       �@��TextAlignment$ Center=�6.   5:      �� 5=   
   "  62   .�5>      ��  � ��$ Center=�6;   5?      5@      .��5B      �  � ColumnDefinitions 5D   
   �� 5E      $��* �6   �� 5G      $	��200 �6   .+�5L   
   &   System.Windows.Controls.DataGrid  -   5          Name$ LagerungDataGrid��5L         AutoGenerateColumns .  
�������   RowBackground
 �����   AlternatingRowBackground
 �����   GridLinesVisibility$
 None=�5N         ColumnHeaderStyle# [ 5O      $��0��5P      $��Stretch=�6#   $��Stretch=�6A    � LayoutTransform 5Q      �� 5R      -   5        	gridScale�� �� ScaleX$ 1Q�5R   4    �� ScaleY$ 1Q�6?      Columns 5U      0  *System.Windows.Controls.DataGridTextColumn  5W      ,  &System.Windows.Controls.DataGridColumn   Header$ Datum��6%      	CellStyle# [  5X   %   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 5Y   %      Binding 5W   4   �� *datum+ �� StringFormat$ yyyy-MM-dd HH:mm:ss��5Y   %     5Z      $ 
Lagerplatz��6%   # [  5[   %   # [ 5\   %    5Z   9   �� *
Lagerplatz+5\   %     5]      $	 Typ��6%   # [  5^   %   # [ 5_   %    5]   2   �� *Typ+5_   %     5`      $ Schablonennummer��6%   # [  5a   %   # [ 5b   %    5`   ?   �� *Schablonennummer+5b   %     5c      $ TOPBOT��6%   # [  5d   %   # [ 5e   %    5c   5   �� *TOPBOT+5e   %     5f      $	 CK#��6%   # [  5g   %   # [ 5h   %    5f   2   �� *CK+5h   %     5i      $ Benutzer��6%   # [  5j   %   # [ 5k   %    5i   7   �� *
Benutzer+5k   %     5l      $ Meldungstext��6%   # [ 5m   %    5l   ;   �� *Meldungstext+5m   %     5n      $ zuletzt benutzt vor:��6%   # [  5o   %   # [ 5p   %    5n   C   ��  �� Path$ ZuletztBenutztVor�5p   %   4  .System.Windows.Controls.DataGridTemplateColumn  5r         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)   # [ 63   5t      �� 5v   
   $��1��6   $��Center=�6%   $��Center=�6@   $��10q�6]   .+�5w      �� -   5        $$��Filtern, Schablonen > 365 Tage��5w      $	��166��6A   $��0,0,0,10q�6M   $��44��6�   �� 5x      -   5        $��Datum filtern��5x      $	��166��6-   $��0,0,0,10q�69   $��44��6q   5y      �� 5|   
   -   5        $��X��5|      $��50��6   $��50��6(   $��Right=�64   $	��Top=�6P    J� RenderTransformOrigin$ 0.5,0.5&�6�   $��1��6�   5~      5      