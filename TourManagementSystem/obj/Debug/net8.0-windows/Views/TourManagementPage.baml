   M S B A M L   `   `   `  ����O  KTourManagementSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:TourManagementSystem.ConvertersTourManagementSystem.Converters  5      3    -TourManagementSystem.Views.TourManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      D
converters-clr-namespace:TourManagementSystem.Converters     5        N� Title$  Tour Management��5       N� 
Background
 ����� � 	Resources 5      >   8TourManagementSystem.Converters.BooleanToStatusConverter   BooleanToStatusConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding	 �� Triggers
 P� Property P� Value  DeleteButton �� BasedOn �� Triggers  
EditButton��%"  &	        &	      &	 b    7  &	 *    7    5	   
   6.   �� 5   
   " ��6%   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 5q�6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 5��5   !   # z��5   !   ._�5      �� $��Center=�6/   $��Center=�5   -   5      	 5      P� 5      
 �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �#1976D26?   5      5       5"      5#      �� 5%   
   " ��6%   8   .�5&      ��  � �� �#F443366+    5'      P� 5(      
 �@J�IsMouseOver$
 True��61   . �5)      ��  � �� �#D32F2F63   5*      5,      �� 5.   
   " ��6#   8   .�5/      ��  � �� �#FF98006+    50      P� 51      
 �@J�IsMouseOver$
 True��61   . �52      ��  � �� �#F57C0063   53      55      56      .��58      � $��20q�6    � RowDefinitions 59   
   �� 5:      $
��Auto �6   �� 5;      $
��Auto �6   �� 5<      $��* �6   .+�5@   
   �� $��0��6   $��0,0,0,20q�6"   .+�5A      �� $��🗺️ Tour Management��6   $��24(�5B      $
��Bold"�5C      $��0,0,0,10q�5D      �� 5E      $.��(Manage tours, schedules, and attractions��6   $��14(�5F      ���#6665G      5H      � 5K   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5L      �� 5M      $��* �6#   �� 5N      $
��Auto �6#   �� 5O      $
��Auto �6#   .+�5S      �� -   5         ��Name$ SearchTextBox��5S      $��0��5T      $��0,0,10,0q�5U      $
��10,8q�5V      $��14(�5W      $��1q�5X      ���#DDD$��Center=�5[      .��66   ��5\      �� 5]      " ��6    5^      P� 5_      
 � ��$  ��66   . �5`   "   ��  � �� 5a   &   !� 5b   *    j� 
AlignmentX$
 Left=�66    j� 
AlignmentY$ Center=�6H    j� Stretch$
 None=�6\    !� Visual 5c   .   �� 5d   2   $��🔍 Search tours...��6<   
�������$��14(�5f   :   5h   +   5j   #   5k      5m      .��5n   !   5o      �� 5r      -   5         ��Name$ StatusFilterComboBox��5r      $��1��5s      $��0,0,10,0q�5t      $
��10,8q�5u      $��14(�5v      $	��150��5w      .@�5y      �� $��
All Status��6    �� 
IsSelected$
 True��64   �� 5z      $��Active��6   �� 5{      $��Inactive��6   �� 5|      $��Draft��6   5}      �� 5�      -   5        $��➕ Add New Tour��5�      $��2��5�      #��[ 5�      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ ToursDataGrid��5�      $��2��5�         AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�         CanUserReorderColumns$
 True��5�         CanUserResizeColumns$
 True��5�         CanUserResizeRows$ False��5�         GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�          SelectionMode$  Single=�5�      !   Columns! 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn"   Header$" ID��6%   #   Width5  /System.Windows.Controls.DataGridLengthConverter$# 60 5�   #   $   
IsReadOnly$
$ True��5�   #   1  +System.Windows.Controls.DataGridBoundColumn%   Binding% 5�   #   �� *TourId+5�   #     5�      $
" Name��6%   $	# 200 5�   #   % 5�   #   �� *
TourName+5�   #     5�      $" Description��6%   $	# 250 5�   #   % 5�   #   �� *Description+5�   #     5�      $" Duration��6%   $	# 100 5�   #   % 5�   #   �� *DurationDays+5�   #     5�      $" Price��6%   $	# 100 5�   #   % 5�   #   �� *	BasePrice+& �� StringFormat$& C��5�   #     5�      $" Destination��6%   $	# 120 5�   #   % 5�   #   �� *Destination+5�   #     5�      $" Status��6%   $	# 100 5�   #   % 5�   #   �� *
IsActive+'   ElementStyle' 5�      �� 5�      " ��6    .�5�      ��  � �� 65   �� *
IsActive+( �� 	Converter#( [   5�      �� 5�   "   ) �� Binding) 6.   �� *
IsActive+* �� Value* True6K   .a�5�   &   ��  � ��
 � � �6C   5�   #   �� 5�   "   ) 6.   �� *
IsActive+	* False6K   .a�5�   &   ��  � ��
 �  ��6C   5�   #   5�      5�        5�      $" Created��6%   $	# 100 5�   #   $
$ True��5�   #   % 5�   #   �� *	CreatedAt+$& 
dd/MM/yyyy��5�   #   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $" Actions��6)   $	# 150 6:   +   CellTemplate+ 5�      �� 5�      .R�5�      �� , �� Orientation$, 
Horizontal=�6)   .+�5�   "   �� -   5        $��✏️��5�   )   #��[ 5�   '   
- � Tag- 5�   '   �� *TourId+�� 5�   "   -   5        $��🗑️��5�   )   #��[ 5�   '   - 5�   '   �� *TourId+5�      5�      5�      5�      5�      5�      