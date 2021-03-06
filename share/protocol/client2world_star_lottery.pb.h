// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: client2world_star_lottery.proto

#ifndef PROTOBUF_client2world_5fstar_5flottery_2eproto__INCLUDED
#define PROTOBUF_client2world_5fstar_5flottery_2eproto__INCLUDED

#include <string>

#include <google/protobuf/stubs/common.h>

#if GOOGLE_PROTOBUF_VERSION < 2006000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please update
#error your headers.
#endif
#if 2006001 < GOOGLE_PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>
#include <google/protobuf/extension_set.h>
#include <google/protobuf/unknown_field_set.h>
#include "client2world_msg_type.pb.h"
#include "msg_type_def.pb.h"
#include "msg_info_def.pb.h"
// @@protoc_insertion_point(includes)

namespace client2world_protocols {

// Internal implementation detail -- do not call these.
void  protobuf_AddDesc_client2world_5fstar_5flottery_2eproto();
void protobuf_AssignDesc_client2world_5fstar_5flottery_2eproto();
void protobuf_ShutdownFile_client2world_5fstar_5flottery_2eproto();

class packetc2w_star_lottery_info;
class packetw2c_star_lottery_info_result;
class packetc2w_req_star_lottery;
class packetc2w_req_star_lottery_result;

// ===================================================================

class packetc2w_star_lottery_info : public ::google::protobuf::Message {
 public:
  packetc2w_star_lottery_info();
  virtual ~packetc2w_star_lottery_info();

  packetc2w_star_lottery_info(const packetc2w_star_lottery_info& from);

  inline packetc2w_star_lottery_info& operator=(const packetc2w_star_lottery_info& from) {
    CopyFrom(from);
    return *this;
  }

  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _unknown_fields_;
  }

  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return &_unknown_fields_;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const packetc2w_star_lottery_info& default_instance();

  void Swap(packetc2w_star_lottery_info* other);

  // implements Message ----------------------------------------------

  packetc2w_star_lottery_info* New() const;
  void CopyFrom(const ::google::protobuf::Message& from);
  void MergeFrom(const ::google::protobuf::Message& from);
  void CopyFrom(const packetc2w_star_lottery_info& from);
  void MergeFrom(const packetc2w_star_lottery_info& from);
  void Clear();
  bool IsInitialized() const;

  int ByteSize() const;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input);
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const;
  ::google::protobuf::uint8* SerializeWithCachedSizesToArray(::google::protobuf::uint8* output) const;
  int GetCachedSize() const { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const;
  public:
  ::google::protobuf::Metadata GetMetadata() const;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_star_lottery_info];
  inline bool has_packet_id() const;
  inline void clear_packet_id();
  static const int kPacketIdFieldNumber = 1;
  inline ::client2world_protocols::e_server_msg_type packet_id() const;
  inline void set_packet_id(::client2world_protocols::e_server_msg_type value);

  // @@protoc_insertion_point(class_scope:client2world_protocols.packetc2w_star_lottery_info)
 private:
  inline void set_has_packet_id();
  inline void clear_has_packet_id();

  ::google::protobuf::UnknownFieldSet _unknown_fields_;

  ::google::protobuf::uint32 _has_bits_[1];
  mutable int _cached_size_;
  int packet_id_;
  friend void  protobuf_AddDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_AssignDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_ShutdownFile_client2world_5fstar_5flottery_2eproto();

  void InitAsDefaultInstance();
  static packetc2w_star_lottery_info* default_instance_;
};
// -------------------------------------------------------------------

class packetw2c_star_lottery_info_result : public ::google::protobuf::Message {
 public:
  packetw2c_star_lottery_info_result();
  virtual ~packetw2c_star_lottery_info_result();

  packetw2c_star_lottery_info_result(const packetw2c_star_lottery_info_result& from);

  inline packetw2c_star_lottery_info_result& operator=(const packetw2c_star_lottery_info_result& from) {
    CopyFrom(from);
    return *this;
  }

  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _unknown_fields_;
  }

  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return &_unknown_fields_;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const packetw2c_star_lottery_info_result& default_instance();

  void Swap(packetw2c_star_lottery_info_result* other);

  // implements Message ----------------------------------------------

  packetw2c_star_lottery_info_result* New() const;
  void CopyFrom(const ::google::protobuf::Message& from);
  void MergeFrom(const ::google::protobuf::Message& from);
  void CopyFrom(const packetw2c_star_lottery_info_result& from);
  void MergeFrom(const packetw2c_star_lottery_info_result& from);
  void Clear();
  bool IsInitialized() const;

  int ByteSize() const;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input);
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const;
  ::google::protobuf::uint8* SerializeWithCachedSizesToArray(::google::protobuf::uint8* output) const;
  int GetCachedSize() const { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const;
  public:
  ::google::protobuf::Metadata GetMetadata() const;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_w2c_star_lottery_info_result];
  inline bool has_packet_id() const;
  inline void clear_packet_id();
  static const int kPacketIdFieldNumber = 1;
  inline ::client2world_protocols::e_server_msg_type packet_id() const;
  inline void set_packet_id(::client2world_protocols::e_server_msg_type value);

  // optional int32 star = 2;
  inline bool has_star() const;
  inline void clear_star();
  static const int kStarFieldNumber = 2;
  inline ::google::protobuf::int32 star() const;
  inline void set_star(::google::protobuf::int32 value);

  // optional int32 award = 3;
  inline bool has_award() const;
  inline void clear_award();
  static const int kAwardFieldNumber = 3;
  inline ::google::protobuf::int32 award() const;
  inline void set_award(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:client2world_protocols.packetw2c_star_lottery_info_result)
 private:
  inline void set_has_packet_id();
  inline void clear_has_packet_id();
  inline void set_has_star();
  inline void clear_has_star();
  inline void set_has_award();
  inline void clear_has_award();

  ::google::protobuf::UnknownFieldSet _unknown_fields_;

  ::google::protobuf::uint32 _has_bits_[1];
  mutable int _cached_size_;
  int packet_id_;
  ::google::protobuf::int32 star_;
  ::google::protobuf::int32 award_;
  friend void  protobuf_AddDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_AssignDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_ShutdownFile_client2world_5fstar_5flottery_2eproto();

  void InitAsDefaultInstance();
  static packetw2c_star_lottery_info_result* default_instance_;
};
// -------------------------------------------------------------------

class packetc2w_req_star_lottery : public ::google::protobuf::Message {
 public:
  packetc2w_req_star_lottery();
  virtual ~packetc2w_req_star_lottery();

  packetc2w_req_star_lottery(const packetc2w_req_star_lottery& from);

  inline packetc2w_req_star_lottery& operator=(const packetc2w_req_star_lottery& from) {
    CopyFrom(from);
    return *this;
  }

  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _unknown_fields_;
  }

  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return &_unknown_fields_;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const packetc2w_req_star_lottery& default_instance();

  void Swap(packetc2w_req_star_lottery* other);

  // implements Message ----------------------------------------------

  packetc2w_req_star_lottery* New() const;
  void CopyFrom(const ::google::protobuf::Message& from);
  void MergeFrom(const ::google::protobuf::Message& from);
  void CopyFrom(const packetc2w_req_star_lottery& from);
  void MergeFrom(const packetc2w_req_star_lottery& from);
  void Clear();
  bool IsInitialized() const;

  int ByteSize() const;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input);
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const;
  ::google::protobuf::uint8* SerializeWithCachedSizesToArray(::google::protobuf::uint8* output) const;
  int GetCachedSize() const { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const;
  public:
  ::google::protobuf::Metadata GetMetadata() const;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_req_star_lottery];
  inline bool has_packet_id() const;
  inline void clear_packet_id();
  static const int kPacketIdFieldNumber = 1;
  inline ::client2world_protocols::e_server_msg_type packet_id() const;
  inline void set_packet_id(::client2world_protocols::e_server_msg_type value);

  // @@protoc_insertion_point(class_scope:client2world_protocols.packetc2w_req_star_lottery)
 private:
  inline void set_has_packet_id();
  inline void clear_has_packet_id();

  ::google::protobuf::UnknownFieldSet _unknown_fields_;

  ::google::protobuf::uint32 _has_bits_[1];
  mutable int _cached_size_;
  int packet_id_;
  friend void  protobuf_AddDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_AssignDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_ShutdownFile_client2world_5fstar_5flottery_2eproto();

  void InitAsDefaultInstance();
  static packetc2w_req_star_lottery* default_instance_;
};
// -------------------------------------------------------------------

class packetc2w_req_star_lottery_result : public ::google::protobuf::Message {
 public:
  packetc2w_req_star_lottery_result();
  virtual ~packetc2w_req_star_lottery_result();

  packetc2w_req_star_lottery_result(const packetc2w_req_star_lottery_result& from);

  inline packetc2w_req_star_lottery_result& operator=(const packetc2w_req_star_lottery_result& from) {
    CopyFrom(from);
    return *this;
  }

  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _unknown_fields_;
  }

  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return &_unknown_fields_;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const packetc2w_req_star_lottery_result& default_instance();

  void Swap(packetc2w_req_star_lottery_result* other);

  // implements Message ----------------------------------------------

  packetc2w_req_star_lottery_result* New() const;
  void CopyFrom(const ::google::protobuf::Message& from);
  void MergeFrom(const ::google::protobuf::Message& from);
  void CopyFrom(const packetc2w_req_star_lottery_result& from);
  void MergeFrom(const packetc2w_req_star_lottery_result& from);
  void Clear();
  bool IsInitialized() const;

  int ByteSize() const;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input);
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const;
  ::google::protobuf::uint8* SerializeWithCachedSizesToArray(::google::protobuf::uint8* output) const;
  int GetCachedSize() const { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const;
  public:
  ::google::protobuf::Metadata GetMetadata() const;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_req_star_lottery_result];
  inline bool has_packet_id() const;
  inline void clear_packet_id();
  static const int kPacketIdFieldNumber = 1;
  inline ::client2world_protocols::e_server_msg_type packet_id() const;
  inline void set_packet_id(::client2world_protocols::e_server_msg_type value);

  // optional .msg_type_def.e_msg_result_def result = 2 [default = e_rmt_fail];
  inline bool has_result() const;
  inline void clear_result();
  static const int kResultFieldNumber = 2;
  inline ::msg_type_def::e_msg_result_def result() const;
  inline void set_result(::msg_type_def::e_msg_result_def value);

  // optional int64 award = 3;
  inline bool has_award() const;
  inline void clear_award();
  static const int kAwardFieldNumber = 3;
  inline ::google::protobuf::int64 award() const;
  inline void set_award(::google::protobuf::int64 value);

  // optional int32 itemtype = 4;
  inline bool has_itemtype() const;
  inline void clear_itemtype();
  static const int kItemtypeFieldNumber = 4;
  inline ::google::protobuf::int32 itemtype() const;
  inline void set_itemtype(::google::protobuf::int32 value);

  // optional int32 itemcount = 5;
  inline bool has_itemcount() const;
  inline void clear_itemcount();
  static const int kItemcountFieldNumber = 5;
  inline ::google::protobuf::int32 itemcount() const;
  inline void set_itemcount(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:client2world_protocols.packetc2w_req_star_lottery_result)
 private:
  inline void set_has_packet_id();
  inline void clear_has_packet_id();
  inline void set_has_result();
  inline void clear_has_result();
  inline void set_has_award();
  inline void clear_has_award();
  inline void set_has_itemtype();
  inline void clear_has_itemtype();
  inline void set_has_itemcount();
  inline void clear_has_itemcount();

  ::google::protobuf::UnknownFieldSet _unknown_fields_;

  ::google::protobuf::uint32 _has_bits_[1];
  mutable int _cached_size_;
  int packet_id_;
  int result_;
  ::google::protobuf::int64 award_;
  ::google::protobuf::int32 itemtype_;
  ::google::protobuf::int32 itemcount_;
  friend void  protobuf_AddDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_AssignDesc_client2world_5fstar_5flottery_2eproto();
  friend void protobuf_ShutdownFile_client2world_5fstar_5flottery_2eproto();

  void InitAsDefaultInstance();
  static packetc2w_req_star_lottery_result* default_instance_;
};
// ===================================================================


// ===================================================================

// packetc2w_star_lottery_info

// optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_star_lottery_info];
inline bool packetc2w_star_lottery_info::has_packet_id() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void packetc2w_star_lottery_info::set_has_packet_id() {
  _has_bits_[0] |= 0x00000001u;
}
inline void packetc2w_star_lottery_info::clear_has_packet_id() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void packetc2w_star_lottery_info::clear_packet_id() {
  packet_id_ = 5060;
  clear_has_packet_id();
}
inline ::client2world_protocols::e_server_msg_type packetc2w_star_lottery_info::packet_id() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_star_lottery_info.packet_id)
  return static_cast< ::client2world_protocols::e_server_msg_type >(packet_id_);
}
inline void packetc2w_star_lottery_info::set_packet_id(::client2world_protocols::e_server_msg_type value) {
  assert(::client2world_protocols::e_server_msg_type_IsValid(value));
  set_has_packet_id();
  packet_id_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_star_lottery_info.packet_id)
}

// -------------------------------------------------------------------

// packetw2c_star_lottery_info_result

// optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_w2c_star_lottery_info_result];
inline bool packetw2c_star_lottery_info_result::has_packet_id() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void packetw2c_star_lottery_info_result::set_has_packet_id() {
  _has_bits_[0] |= 0x00000001u;
}
inline void packetw2c_star_lottery_info_result::clear_has_packet_id() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void packetw2c_star_lottery_info_result::clear_packet_id() {
  packet_id_ = 7564;
  clear_has_packet_id();
}
inline ::client2world_protocols::e_server_msg_type packetw2c_star_lottery_info_result::packet_id() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetw2c_star_lottery_info_result.packet_id)
  return static_cast< ::client2world_protocols::e_server_msg_type >(packet_id_);
}
inline void packetw2c_star_lottery_info_result::set_packet_id(::client2world_protocols::e_server_msg_type value) {
  assert(::client2world_protocols::e_server_msg_type_IsValid(value));
  set_has_packet_id();
  packet_id_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetw2c_star_lottery_info_result.packet_id)
}

// optional int32 star = 2;
inline bool packetw2c_star_lottery_info_result::has_star() const {
  return (_has_bits_[0] & 0x00000002u) != 0;
}
inline void packetw2c_star_lottery_info_result::set_has_star() {
  _has_bits_[0] |= 0x00000002u;
}
inline void packetw2c_star_lottery_info_result::clear_has_star() {
  _has_bits_[0] &= ~0x00000002u;
}
inline void packetw2c_star_lottery_info_result::clear_star() {
  star_ = 0;
  clear_has_star();
}
inline ::google::protobuf::int32 packetw2c_star_lottery_info_result::star() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetw2c_star_lottery_info_result.star)
  return star_;
}
inline void packetw2c_star_lottery_info_result::set_star(::google::protobuf::int32 value) {
  set_has_star();
  star_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetw2c_star_lottery_info_result.star)
}

// optional int32 award = 3;
inline bool packetw2c_star_lottery_info_result::has_award() const {
  return (_has_bits_[0] & 0x00000004u) != 0;
}
inline void packetw2c_star_lottery_info_result::set_has_award() {
  _has_bits_[0] |= 0x00000004u;
}
inline void packetw2c_star_lottery_info_result::clear_has_award() {
  _has_bits_[0] &= ~0x00000004u;
}
inline void packetw2c_star_lottery_info_result::clear_award() {
  award_ = 0;
  clear_has_award();
}
inline ::google::protobuf::int32 packetw2c_star_lottery_info_result::award() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetw2c_star_lottery_info_result.award)
  return award_;
}
inline void packetw2c_star_lottery_info_result::set_award(::google::protobuf::int32 value) {
  set_has_award();
  award_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetw2c_star_lottery_info_result.award)
}

// -------------------------------------------------------------------

// packetc2w_req_star_lottery

// optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_req_star_lottery];
inline bool packetc2w_req_star_lottery::has_packet_id() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void packetc2w_req_star_lottery::set_has_packet_id() {
  _has_bits_[0] |= 0x00000001u;
}
inline void packetc2w_req_star_lottery::clear_has_packet_id() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void packetc2w_req_star_lottery::clear_packet_id() {
  packet_id_ = 5061;
  clear_has_packet_id();
}
inline ::client2world_protocols::e_server_msg_type packetc2w_req_star_lottery::packet_id() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery.packet_id)
  return static_cast< ::client2world_protocols::e_server_msg_type >(packet_id_);
}
inline void packetc2w_req_star_lottery::set_packet_id(::client2world_protocols::e_server_msg_type value) {
  assert(::client2world_protocols::e_server_msg_type_IsValid(value));
  set_has_packet_id();
  packet_id_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery.packet_id)
}

// -------------------------------------------------------------------

// packetc2w_req_star_lottery_result

// optional .client2world_protocols.e_server_msg_type packet_id = 1 [default = e_mst_c2w_req_star_lottery_result];
inline bool packetc2w_req_star_lottery_result::has_packet_id() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void packetc2w_req_star_lottery_result::set_has_packet_id() {
  _has_bits_[0] |= 0x00000001u;
}
inline void packetc2w_req_star_lottery_result::clear_has_packet_id() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void packetc2w_req_star_lottery_result::clear_packet_id() {
  packet_id_ = 7565;
  clear_has_packet_id();
}
inline ::client2world_protocols::e_server_msg_type packetc2w_req_star_lottery_result::packet_id() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery_result.packet_id)
  return static_cast< ::client2world_protocols::e_server_msg_type >(packet_id_);
}
inline void packetc2w_req_star_lottery_result::set_packet_id(::client2world_protocols::e_server_msg_type value) {
  assert(::client2world_protocols::e_server_msg_type_IsValid(value));
  set_has_packet_id();
  packet_id_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery_result.packet_id)
}

// optional .msg_type_def.e_msg_result_def result = 2 [default = e_rmt_fail];
inline bool packetc2w_req_star_lottery_result::has_result() const {
  return (_has_bits_[0] & 0x00000002u) != 0;
}
inline void packetc2w_req_star_lottery_result::set_has_result() {
  _has_bits_[0] |= 0x00000002u;
}
inline void packetc2w_req_star_lottery_result::clear_has_result() {
  _has_bits_[0] &= ~0x00000002u;
}
inline void packetc2w_req_star_lottery_result::clear_result() {
  result_ = 2;
  clear_has_result();
}
inline ::msg_type_def::e_msg_result_def packetc2w_req_star_lottery_result::result() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery_result.result)
  return static_cast< ::msg_type_def::e_msg_result_def >(result_);
}
inline void packetc2w_req_star_lottery_result::set_result(::msg_type_def::e_msg_result_def value) {
  assert(::msg_type_def::e_msg_result_def_IsValid(value));
  set_has_result();
  result_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery_result.result)
}

// optional int64 award = 3;
inline bool packetc2w_req_star_lottery_result::has_award() const {
  return (_has_bits_[0] & 0x00000004u) != 0;
}
inline void packetc2w_req_star_lottery_result::set_has_award() {
  _has_bits_[0] |= 0x00000004u;
}
inline void packetc2w_req_star_lottery_result::clear_has_award() {
  _has_bits_[0] &= ~0x00000004u;
}
inline void packetc2w_req_star_lottery_result::clear_award() {
  award_ = GOOGLE_LONGLONG(0);
  clear_has_award();
}
inline ::google::protobuf::int64 packetc2w_req_star_lottery_result::award() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery_result.award)
  return award_;
}
inline void packetc2w_req_star_lottery_result::set_award(::google::protobuf::int64 value) {
  set_has_award();
  award_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery_result.award)
}

// optional int32 itemtype = 4;
inline bool packetc2w_req_star_lottery_result::has_itemtype() const {
  return (_has_bits_[0] & 0x00000008u) != 0;
}
inline void packetc2w_req_star_lottery_result::set_has_itemtype() {
  _has_bits_[0] |= 0x00000008u;
}
inline void packetc2w_req_star_lottery_result::clear_has_itemtype() {
  _has_bits_[0] &= ~0x00000008u;
}
inline void packetc2w_req_star_lottery_result::clear_itemtype() {
  itemtype_ = 0;
  clear_has_itemtype();
}
inline ::google::protobuf::int32 packetc2w_req_star_lottery_result::itemtype() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery_result.itemtype)
  return itemtype_;
}
inline void packetc2w_req_star_lottery_result::set_itemtype(::google::protobuf::int32 value) {
  set_has_itemtype();
  itemtype_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery_result.itemtype)
}

// optional int32 itemcount = 5;
inline bool packetc2w_req_star_lottery_result::has_itemcount() const {
  return (_has_bits_[0] & 0x00000010u) != 0;
}
inline void packetc2w_req_star_lottery_result::set_has_itemcount() {
  _has_bits_[0] |= 0x00000010u;
}
inline void packetc2w_req_star_lottery_result::clear_has_itemcount() {
  _has_bits_[0] &= ~0x00000010u;
}
inline void packetc2w_req_star_lottery_result::clear_itemcount() {
  itemcount_ = 0;
  clear_has_itemcount();
}
inline ::google::protobuf::int32 packetc2w_req_star_lottery_result::itemcount() const {
  // @@protoc_insertion_point(field_get:client2world_protocols.packetc2w_req_star_lottery_result.itemcount)
  return itemcount_;
}
inline void packetc2w_req_star_lottery_result::set_itemcount(::google::protobuf::int32 value) {
  set_has_itemcount();
  itemcount_ = value;
  // @@protoc_insertion_point(field_set:client2world_protocols.packetc2w_req_star_lottery_result.itemcount)
}


// @@protoc_insertion_point(namespace_scope)

}  // namespace client2world_protocols

#ifndef SWIG
namespace google {
namespace protobuf {


}  // namespace google
}  // namespace protobuf
#endif  // SWIG

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_client2world_5fstar_5flottery_2eproto__INCLUDED
