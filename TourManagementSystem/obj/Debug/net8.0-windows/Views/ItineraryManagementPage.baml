   M S B A M L   `   `   `  ����O  KTourManagementSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:TourManagementSystem.ConvertersTourManagementSystem.Converters  5      8    2TourManagementSystem.Views.ItineraryManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      D
converters-clr-namespace:TourManagementSystem.Converters     5      -   5          N� Title$  Itinerary Management��5       N� 
Background
 ����� � 	Resources 5	      >   8TourManagementSystem.Converters.BooleanToStatusConverter   BooleanToStatusConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� Padding �� CornerRadius	 �� Triggers
 P� Property P� Value  ActionButtonSmall �� BasedOn  DangerButton �� Triggers  
EditButton��%L  &	        &	      &	 �    7  &	 ~    7  &	 F    7    5
   
   6.   �� 5   
   " ��6%   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$ SemiBold"�6+   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   # z��5   !   $ 4��5   !   ._�5      �� $��Center=�6/   $��Center=�5   -   5      	 5      P� 5      
 �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �#1976D26?   5       P� 5!      
 � ��$
 True��6;   . �5"   "   ��  � �� �#0D47A16?   5#      5%      5'      5(      �� 5*   
   " ��6*   8   .�5+      ��  � ��$	 8,4q�6(   �� 5,       � ��$ 12(�6)   �� 5-       � ��$ 2q�6'   5.      �� 50   
   " ��6%   8   .�51      ��  � �� �#F443366+    52      P� 53      
 �@J�IsMouseOver$
 True��61   . �54      ��  � �� �#D32F2F63   55      57      �� 59   
   " ��6#   8   .�5:      ��  � �� �#FF98006+    5;      P� 5<      
 �@J�IsMouseOver$
 True��61   . �5=      ��  � �� �#F57C0063   5>      5@      5A      .��5C      � $��20q�6    � RowDefinitions 5D   
   �� 5E      $
��Auto �6   �� 5F      $
��Auto �6   �� 5G      $��* �6   .+�5K   
   �� $��0��6   $��0,0,0,20q�6"   .+�5L      �� $"��🗺️ Itinerary Management��6   $��24(�5M      $
��Bold"�5N      ���#3335O      �� 5P      $-��'Manage tour itineraries and attractions��6   $��14(�5Q      ���#666$��0,5,0,0q�5S      5T      � 5W   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5X      �� 5Y      $��* �6#   �� 5Z      $
��Auto �6#   �� 5[      $
��Auto �6#   .+�5_      �� -   5         ��Name$ SearchTextBox��5_      $��0��5`      $��0,0,10,0q�5a      $
��10,8q�5b      $��14(�5c      $��1q�5d      ���#DDD$��Center=�5g      .��66   ��5h      �� 5i      " ��6    5j      P� 5k      
 � ��$  ��66   . �5l   "   ��  � �� 5m   &   !� 5n   *    j� 
AlignmentX$
 Left=�66    j� 
AlignmentY$ Center=�6H    j� Stretch$
 None=�6\    !� Visual 5o   .   �� 5p   2   $ ��🔍 Search itineraries...��6<   
�������$��14(�5r   :   5t   +   5v   #   5w      5y      .��5z   !   5{      �� 5~      -   5         ��Name$ TourFilterComboBox��5~      $��1��5      $��0,0,10,0q�5�      $
��10,8q�5�      $��14(�5�      $	��200��5�      .@�5�      �� $��	All Tours��6    �� 
IsSelected$
 True��63   5�      �� 5�      -   5        $��➕ Add Itinerary��5�      $��2��5�      #��[ 5�      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ ItinerariesDataGrid��5�      $��2��5�         AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         CanUserReorderColumns$
 True��5�         CanUserResizeColumns$
 True��5�         CanUserResizeRows$ False��5�         GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�          SelectionMode$  Single=�5�      !   Columns! 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn"   Header$
" Tour��6%   #   Width5  /System.Windows.Controls.DataGridLengthConverter$	# 200 5�   #   1  +System.Windows.Controls.DataGridBoundColumn$   Binding$ 5�   #   �� *Tour.TourName+5�   #     5�      $" 
Attraction��6%   $	# 200 5�   #   $ 5�   #   �� *Attraction.AttractionName+5�   #     5�      $" Location��6%   $	# 150 5�   #   $ 5�   #   �� *Attraction.Location+5�   #     5�      $" 	Visit Day��6%   $# 80 5�   #   $ 5�   #   �� *
VisitDay+5�   #     5�      $" Visit Order��6%   $# 80 5�   #   $ 5�   #   �� *
VisitOrder+5�   #     5�      $" Description��6%   $	# 250 5�   #   $ 5�   #   �� *Description+5�   #   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $" Actions��6)   $	# 150 6:   %   CellTemplate% 5�      �� 5�      .R�5�      �� & �� Orientation$& 
Horizontal=�6)   .+�5�   "   �� -   5        $��✏️ Edit��5�   )   #��[ 5�   '   
' � Tag' 5�   '   �� �� 5�   "   -   5        $��🗑️ Delete��5�   )   #��[ 5�   '   ' 5�   '   �� 5�      5�      5�      5�      5�      5�      