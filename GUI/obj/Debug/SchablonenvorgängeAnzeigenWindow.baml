   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	   +    %GUI.SchablonenvorgängeAnzeigenWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	   -   5          � Title$'  !SchablonenvorgängeAnzeigenWindow��5   	   $	��450��62   $	��800��6?    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�6!    � 	Resources 5         DefaultCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value  MeldungstextCellStyle �� Triggers P� Property	 P� Value  NoRightBorderCellStyle��%.  &	        &	 y     &	 �    �� 5   
   "  6)   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0.5,0.5q�60   5      �� 5   
   "  6.   .�5      ��  � ��
 �   �6,   �� 5       � ��$ 0,0.5,0,0.5q�60   �� 5       � ��
 ��d�6+   �� 5       � ��
 �   �6+    5      P� 5       �@ 
IsSelected	 . . �5      ��  � ��
 �����63   5      5      �� 5   
   "  6/   .�5      ��  � ��
 ���� 6,   �� 5       � ��$ 0q�60   5      5      .��5       � 
 � LayoutTransform
 5"   
   �� 5#      -   5        windowScale�� �� ScaleX$ 1Q�5#   2    �� ScaleY$ 1Q�6=   .+�5&   
   &   System.Windows.Controls.DataGrid  -   5          Name$! SchablonenvorgaengeDataGrid��5&         AutoGenerateColumns .  $��Stretch=�6T   $��Stretch=�6r   
�������   RowBackground
 �����   GridLinesVisibility$
 None=�5'   <      Columns 5(      0  *System.Windows.Controls.DataGridTextColumn  5)      ,  &System.Windows.Controls.DataGridColumn   Header$ Datum��6%      	CellStyle# [  6N   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *datum+6N     5*      $ 
Lagerplatz��6%   # [  6X    69   �� *
Lagerplatz+6X     5+      $	 Typ��6%   # [  6J    62   �� *Typ+6J     5,      $ Schablonennummer��6%   # [  6d    6?   �� *Schablonennummer+6d     5-      $ TOP/BOT��6%   # [  6Q    66   �� *TOPBOT+6Q     5.      $	 CK#��6%   # [  6J    62   �� *CK#+6J     5/      $ Benutzer��6%   # [  6T    67   �� *
Benutzer+6T   4  .System.Windows.Controls.DataGridTemplateColumn  50      $ Meldungstext��6)   # [ 6?      CellTemplate 51      �� 52      .R�53      �� ��6(   �� *Meldungstext+54      56        57         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)   # [ 63   59      �� 5<   
   -   5        $��X��5<      $��50��6   $��50��6(   $��Right=�64   $	��Top=�6P    J� RenderTransformOrigin$ 0.5,0.5&�6�   5>      5?      