   M S B A M L   `   `   `  ����  TourManagementSystem2    ,TourManagementSystem.Views.AddScheduleWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Create Schedule��5   	   $	��652��5   	   $	��524��6    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	   ���#F5F5F5 � 	Resources 5
         FormTextBox �� 
TargetType �� Property �� Value  FormComboBox  ActionButton �� 
TargetType �� CornerRadius	 �� Padding
 �� Triggers P� Property P� Value  CancelButton �� BasedOn �� Triggers��%�  &	        &	 
    &	     &	 �    7  �� 5   
   " ��6$   .�5      ��  � ��$
 10,8q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$ 1q�60   �� 5       � �� �#DDD6,   �� 5       � ��$ 0,5,0,15q�6'   �� 5       � ��$ Center=�69   5      �� 5   
   " ��6%   .�5      ��  � ��$
 10,8q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$ 1q�60   �� 5       � �� �#DDD6,   �� 5       � ��$ 0,5,0,15q�6'   �� 5       � ��$ Center=�69   5      �� 5   
   " ��6%   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5        � ��$ 0q�60   �� 5!       � ��$ 20,10q�6(   �� 5"       � ��$ 14(�6)   �� 5#       � ��$ SemiBold"�6+   �� 5$       �@�Cursor$
 Hand��6'   �� 5%       � �� 5&      �� 5'      " ��6&   .R�5(      �� #��z��6!   $ 5��5)   !   #	 z��5*   !   ._�5+      �� $��Center=�6/   $��Center=�5,   -   5-      
 5.      P� 5/       �@J�IsMouseOver$
 True��6=   . �50   "   ��  � �� �#1976D26?   51      53      55      56      �� 58   
   " ��6%   8   .�59      ��  � �� �#7575756+    5:      P� 5;       �@J�IsMouseOver$
 True��61   . �5<      ��  � �� �#61616163   5=      5?      5@      .��5B      � $��20q�6    � RowDefinitions 5C   
   �� 5D      $
��Auto �6   �� 5E      $��* �6   �� 5F      $
��Auto �6   .+�5J   
   �� $��0��6   $��0,0,0,20q�6"   .+�5K      �� $��📅 Create Schedule��6   $��24(�5L      $
��Bold"�5M      $��0,0,0,5q�5N      �� 5O      $ ��Create a new tour schedule��6   $��14(�5P      ���#6665Q      5R      �� 5U   
   $��1��6   .+�5W      �� $��Tour *��6   $��14(�5X      $��SemiBold"�5Y      �� 5Z      -   5         ��Name$ TourComboBox��5Z      #��[ 5[       �� DisplayMemberPath$
 Name��5\      5]      �� 5`      $��Departure Date *��6   $��14(�5a      $��SemiBold"�5b      (  "System.Windows.Controls.DatePicker  5c      -   5          Name$ DepartureDatePicker��5c      $��0,5,0,15q�5d      $��14(�5e      �� 5h      $��Return Date *��6   $��14(�5i      $��SemiBold"�5j        5k      -   5        $ ReturnDatePicker��5k      $��0,5,0,15q�5l      $��14(�5m      �� 5p      $��Max Capacity *��6   $��14(�5q      $��SemiBold"�5r      �� 5s      -   5         ��Name$ MaxCapacityTextBox��5s      #��[  5t       �� 	MaxLength$ 3��5u      �� 5x      $��Guide *��6   $��14(�5y      $��SemiBold"�5z      �� 5{      -   5        $ GuideComboBox��5{      #��[ 5|      $ FullName��5}      �� 5�      $��Status��6   $��14(�5�      $��SemiBold"�5�      �� 5�      -   5        $ StatusComboBox��5�      #��[ 5�      .@�5�      �� $��	Scheduled��6    �� 
IsSelected$
 True��63   �� 5�      $��In Progress��6   �� 5�      $��	Completed��6   �� 5�      $��	Cancelled��6   5�      5�      �� 5�   
   $��2��6    �� Orientation$ 
Horizontal=�5�      $��Right=�5�      $��0,20,0,0q�5�      .+�5�      �� -   5        $��Cancel��5�      #��[ 5�      $��0,0,10,0q�5�      �� 5�      -   5        $��Create Schedule��5�      #��[ 5�      5�      5�      5�      5�      