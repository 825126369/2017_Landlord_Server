#pragma once
#include <net\packet_manager.h>
#include <game_landlord_net_human_protocol.pb.h>
#include <net\peer_tcp.h>

class i_game_player;
using namespace game_landlord_net_human_protocol;

void init_proc_landlord_protocol();

//��������/��������
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_create_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_create_room_result);

//�й�
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_Hang, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_Hang_result);

//������ӻ�����
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_add_robot, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_add_robot_result);

//�߳�����
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_kich_player, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_kich_player_result);

//��������б�
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_join_player_list, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_join_player_list_result);

//�������
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_join_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_join_room_result);

//�Ƿ�ͬ������
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_or_agree_join_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_or_agree_join_room_result);

//׼��
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_or_prepare, i_game_player);

//ϵͳ����
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_SystemTalk, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_notice_SystemTalk);

//��ȡ�����б���
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_roomlist, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_roomlist_result);

//���뷿��
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_enter_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_enter_room_result);

//�뿪����
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_leave_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_leave_room_result);

//�������� ������Ϣ
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_room_scene_info, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_get_room_scene_info_result);

//�����Ϸ״̬		//����Ϸ֮���ж��Ƿ��ڶ�����
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_check_state, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_check_state_result);

//�������
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_playhand, i_game_player);

//������
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_rob_landlord, i_game_player);

//֪ͨ��ʼ��Ϸ
PACKET_REGEDIT_SEND(packetl2c_notice_startgame);
//֪ͨ ĳĳ�� ������Ϣ
PACKET_REGEDIT_SEND(packetl2c_notice_who_playhand);
//֪ͨ �����˳�����Ϣ
PACKET_REGEDIT_SEND(packetl2c_notice_playhand);
//֪ͨ �Ƿ���Ҫ������
PACKET_REGEDIT_SEND(packetl2c_notice_rob_landlord);
//֪ͨ ʤ��
PACKET_REGEDIT_SEND(packetl2c_notice_winlose);
//֪ͨ������
PACKET_REGEDIT_SEND(packetl2c_notice_rob_landlord_result);

//֪ͨ������ ĳ��ҽ������
PACKET_REGEDIT_SEND(packetl2c_notice_room_enter_leave_info);
//֪ͨ������ ĳ���׼����Ϣ
PACKET_REGEDIT_SEND(packetl2c_notice_room_prepare_info);
//֪ͨ ������ ������Ϣ
PACKET_REGEDIT_SEND(packetl2c_notice_room_change_info);

//֪ͨ ������뷿����Ϣ�б�
PACKET_REGEDIT_SEND(packetl2c_notice_invite_room_list);

//֪ͨ �����ı���Ϣ
PACKET_REGEDIT_SEND(packetl2c_notice_who_is_roomcreator);




