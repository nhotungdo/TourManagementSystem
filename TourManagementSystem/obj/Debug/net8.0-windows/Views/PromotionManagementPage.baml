   M S B A M L   `   `   `  ����  TourManagementSystem8    2TourManagementSystem.Views.PromotionManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� Title$  Promotion Management��5       N� 
Background
 ����� � 	Resources 5         ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding	 �� Triggers
 P� Property P� Value  DeleteButton �� BasedOn �� Triggers  
EditButton��%  &	        &	 O    7   &	     7   �� 5   
   " ��6%   .�5	      ��  � �� �#2196F36+   �� 5
       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 5q�6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 5��5   !   # z��5   !   ._�5      �� $��Center=�6/   $��Center=�5   -   5      	 5      P� 5      
 �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �#1976D26?   5      5      5      5       �� 5"   
   " ��6%   8   .�5#      ��  � �� �#F443366+    5$      P� 5%      
 �@J�IsMouseOver$
 True��61   . �5&      ��  � �� �#D32F2F63   5'      5)      �� 5+   
   " ��6#   8   .�5,      ��  � �� �#FF98006+    5-      P� 5.      
 �@J�IsMouseOver$
 True��61   . �5/      ��  � �� �#F57C0063   50      52      53      .��55      � $��20q�6    � RowDefinitions 56   
   �� 57      $
��Auto �6   �� 58      $
��Auto �6   �� 59      $��* �6   .+�5=   
   �� $��0��6   $��0,0,0,20q�6"   .+�5>      �� $��🎁 Promotion Management��6   $��24(�5?      $
��Bold"�5@      $��0,0,0,10q�5A      �� 5B      $*��$Create and manage promotional offers��6   $��14(�5C      ���#6665D      5E      � 5H   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5I      �� 5J      $��* �6#   �� 5K      $
��Auto �6#   �� 5L      $
��Auto �6#   .+�5P      �� -   5         ��Name$ SearchTextBox��5P      $��0��5Q      $��0,0,10,0q�5R      $
��10,8q�5S      $��14(�5T      $��1q�5U      ���#DDD$��Center=�5X      .��66   ��5Y      �� 5Z      " ��6    5[      P� 5\      
 � ��$  ��66   . �5]   "   ��  � �� 5^   &   !� 5_   *    j� 
AlignmentX$
 Left=�66    j� 
AlignmentY$ Center=�6H    j� Stretch$
 None=�6\    !� Visual 5`   .   �� 5a   2   $��🔍 Search promotions...��6<   
�������$��14(�5c   :   5e   +   5g   #   5h      5j      .��5k   !   5l      �� 5o      -   5         ��Name$ StatusFilterComboBox��5o      $��1��5p      $��0,0,10,0q�5q      $
��10,8q�5r      $��14(�5s      $	��150��5t      .@�5v      �� $��
All Status��6    �� 
IsSelected$
 True��64   �� 5w      $��Active��6   �� 5x      $��Inactive��6   �� 5y      $��Expired��6   5z      �� 5}      -   5        $��➕ Add Promotion��5}      $��2��5~      #��[  5      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ PromotionsDataGrid��5�      $��2��5�         AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         CanUserReorderColumns$
 True��5�         CanUserResizeColumns$
 True��5�         CanUserResizeRows$ False��5�         GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�          SelectionMode$  Single=�5�      !   Columns! 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn"   Header$" ID��6%   #   Width5  /System.Windows.Controls.DataGridLengthConverter$# 60 5�   #   $   
IsReadOnly$
$ True��5�   #   1  +System.Windows.Controls.DataGridBoundColumn%   Binding% 5�   #   �� *PromotionId+5�   #     5�      $
" Code��6%   $	# 120 5�   #   % 5�   #   �� *PromotionCode+5�   #     5�      $
" Name��6%   $	# 150 5�   #   % 5�   #   �� *PromotionName+5�   #     5�      $" Description��6%   $	# 200 5�   #   % 5�   #   �� *Description+5�   #     5�      $" 
Discount %��6%   $	# 100 5�   #   % 5�   #   �� *DiscountPercentage+5�   #     5�      $" Discount Amount��6%   $	# 120 5�   #   % 5�   #   �� *DiscountAmount+& �� StringFormat$& C��5�   #     5�      $" 
Min Amount��6%   $	# 120 5�   #   % 5�   #   �� *MinBookingAmount+$& C��5�   #     5�      $" 
Start Date��6%   $	# 120 5�   #   % 5�   #   �� *	StartDate+$& 
dd/MM/yyyy��5�   #     5�      $" End Date��6%   $	# 120 5�   #   % 5�   #   �� *	EndDate+$& 
dd/MM/yyyy��5�   #     5�      $" Status��6%   $	# 100 5�   #   % 5�   #   �� *
IsActive+'   ElementStyle' 5�      �� 5�      " ��6    .�5�      ��  � �� 65   �� *
IsActive+ 5�      �� 5�   "   ( �� Binding( 6.   �� *
IsActive+) �� Value) True6K   .a�5�   &   ��  � ��$ Active��6=   �� 5�   &    � ��
 � � �6C   5�   #   �� 5�   "   ( 6.   �� *
IsActive+	) False6K   .a�5�   &   ��  � ��$ Inactive��6=   �� 5�   &    � ��
 �  ��6C   5�   #   5�      5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      $" Actions��6)   $	# 150 6:   *   CellTemplate* 5�      �� 5�      .R�5�      �� + �� Orientation$+ 
Horizontal=�6)   .+�5�   "   �� -   5        $��✏️��5�   )   #��[ 5�   '   
, � Tag, 5�   '   �� *PromotionId+�� 5�   "   -   5        $��🗑️��5�   )   #��[ 5�   '   , 5�   '   �� *PromotionId+5�      5�      5�      5�      5�      5�      