   M S B A M L   `   `   `  ����O  KTourManagementSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:TourManagementSystem.ConvertersTourManagementSystem.Converters  5      9    3TourManagementSystem.Views.AttractionManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      D
converters-clr-namespace:TourManagementSystem.Converters     5        N� Title$  Attraction Management��5       N� 
Background
 ����� � 	Resources 5      >   8TourManagementSystem.Converters.BooleanToStatusConverter   BooleanToStatusConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� Padding �� CornerRadius	 �� Triggers
 P� Property P� Value  ActionButtonSmall �� BasedOn  DangerButton �� Triggers  
EditButton��%L  &	        &	      &	 �    7  &	 ~    7  &	 F    7    5	   
   6.   �� 5   
   " ��6%   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$ SemiBold"�6+   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   # z��5   !   $ 4��5   !   ._�5      �� $��Center=�6/   $��Center=�5   -   5      	 5      P� 5      
 �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �#1976D26?   5      P� 5       
 � ��$
 True��6;   . �5!   "   ��  � �� �#0D47A16?   5"      5$      5&      5'      �� 5)   
   " ��6*   8   .�5*      ��  � ��$	 8,4q�6(   �� 5+       � ��$ 12(�6)   �� 5,       � ��$ 2q�6'   5-      �� 5/   
   " ��6%   8   .�50      ��  � �� �#F443366+    51      P� 52      
 �@J�IsMouseOver$
 True��61   . �53      ��  � �� �#D32F2F63   54      56      �� 58   
   " ��6#   8   .�59      ��  � �� �#FF98006+    5:      P� 5;      
 �@J�IsMouseOver$
 True��61   . �5<      ��  � �� �#F57C0063   5=      5?      5@      .��5B      � $��20q�6    � RowDefinitions 5C   
   �� 5D      $
��Auto �6   �� 5E      $
��Auto �6   �� 5F      $��* �6   .+�5J   
   �� $��0��6   $��0,0,0,20q�6"   .+�5K      �� $#��🏞️ Attraction Management��6   $��24(�5L      $
��Bold"�5M      ���#3335N      �� 5O      $1��+Manage tourist attractions and destinations��6   $��14(�5P      ���#666$��0,5,0,0q�5R      5S      � 5V   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5W      �� 5X      $��* �6#   �� 5Y      $
��Auto �6#   �� 5Z      $
��Auto �6#   .+�5^      �� -   5         ��Name$ SearchTextBox��5^      $��0��5_      $��0,0,10,0q�5`      $
��10,8q�5a      $��14(�5b      .��5c   <   ��5d      �� 5e      " ��6   .�5f      ��  � �� 5g      �� 5h   "   " ��62   .R�5i   &   �� 
����������#E0E0E0$��1q�5k   -   $ 4��5l   -   ._�5m   *   � .+�5n   .   �� 
������ $��0q�5p   4   $
��10,8q�5q   4   $��14(�5r   4   $��Center=�5s   4   ��5n   6   ��  �� RelativeSource �� *TemplatedParent+ �� Path$
 Text� �� UpdateSourceTrigger$ PropertyChanged=�5s   4   �� 5t   .   $��Search attractions...��68   ���#999$��10,8,0,0q�5v   6   $��Center=�5w   6    J� IsHitTestVisible$ False��5x   6   ��5y   2   �� 5z   6   " ��6<   .�5{   :   ��  � y�$ 	Collapsed=�6W    5|   :   �� 5}   >    �� Binding 6J   �� *Text+ �� ElementName$ SearchTextBox�� �� Value  6~   .a�5~   B   ��  � y�$ Visible=�6_   5   ?   5�   7   5�   /   5�   +   5�   '   5�   #   5�      5�      .��5�   !   5�      �� 5�      -   5         ��Name$ LocationFilterComboBox��5�      $��1��5�      $��0,0,10,0q�5�      $
��10,8q�5�      $��14(�5�      $	��200��5�      .@�5�      �� $��All Locations��6    �� 
IsSelected$
 True��67   5�      �� 5�      -   5        $��➕ Add Attraction��5�      $��2��5�      #��[ 5�      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ AttractionsDataGrid��5�      $��2��5�         AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         CanUserReorderColumns$
 True��5�         CanUserResizeColumns$
 True��5�          CanUserResizeRows$  False��5�      !   GridLinesVisibility$! 
Horizontal=�5�      "   HeadersVisibility$" Column=�5�      #   SelectionMode$# Single=�5�      $   Columns$ 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn%   Header$% ID��6%   &   Width5  /System.Windows.Controls.DataGridLengthConverter$& 60 5�   #   '   
IsReadOnly$
' True��5�   #   1  +System.Windows.Controls.DataGridBoundColumn(   Binding( 5�   #   �� *AttractionId+5�   #     5�      $
% Name��6%   $	& 200 5�   #   ( 5�   #   �� *AttractionName+5�   #     5�      $% Location��6%   $	& 150 5�   #   ( 5�   #   �� *
Location+5�   #     5�      $% Description��6%   $	& 300 5�   #   ( 5�   #   �� *Description+5�   #     5�      $% 	Image URL��6%   $	& 200 5�   #   ( 5�   #   �� *
ImageUrl+5�   #   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $% Actions��6)   $	& 150 6:   )   CellTemplate) 5�      �� 5�      .R�5�      �� * �� Orientation$* 
Horizontal=�6)   .+�5�   "   �� -   5        $��✏️ Edit��5�   )   #��[ 5�   '   
+ � Tag+ 5�   '   �� *AttractionId+�� 5�   "   -   5        $��🗑️ Delete��5�   )   #��[ 5�   '   + 5�   '   �� *AttractionId+5�      5�      5�      5�      5�      5�      