// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: server_base.proto

#define INTERNAL_SUPPRESS_PROTOBUF_FIELD_DEPRECATION
#include "server_base.pb.h"

#include <algorithm>

#include <google/protobuf/stubs/common.h>
#include <google/protobuf/stubs/once.h>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/wire_format_lite_inl.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/reflection_ops.h>
#include <google/protobuf/wire_format.h>
// @@protoc_insertion_point(includes)

namespace server_protocols {

namespace {

const ::google::protobuf::Descriptor* server_attributes_descriptor_ = NULL;
const ::google::protobuf::internal::GeneratedMessageReflection*
  server_attributes_reflection_ = NULL;
const ::google::protobuf::Descriptor* server_info_descriptor_ = NULL;
const ::google::protobuf::internal::GeneratedMessageReflection*
  server_info_reflection_ = NULL;
const ::google::protobuf::Descriptor* msg_packet_descriptor_ = NULL;
const ::google::protobuf::internal::GeneratedMessageReflection*
  msg_packet_reflection_ = NULL;
const ::google::protobuf::EnumDescriptor* e_server_type_descriptor_ = NULL;

}  // namespace


void protobuf_AssignDesc_server_5fbase_2eproto() {
  protobuf_AddDesc_server_5fbase_2eproto();
  const ::google::protobuf::FileDescriptor* file =
    ::google::protobuf::DescriptorPool::generated_pool()->FindFileByName(
      "server_base.proto");
  GOOGLE_CHECK(file != NULL);
  server_attributes_descriptor_ = file->message_type(0);
  static const int server_attributes_offsets_[1] = {
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_attributes, client_count_),
  };
  server_attributes_reflection_ =
    new ::google::protobuf::internal::GeneratedMessageReflection(
      server_attributes_descriptor_,
      server_attributes::default_instance_,
      server_attributes_offsets_,
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_attributes, _has_bits_[0]),
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_attributes, _unknown_fields_),
      -1,
      ::google::protobuf::DescriptorPool::generated_pool(),
      ::google::protobuf::MessageFactory::generated_factory(),
      sizeof(server_attributes));
  server_info_descriptor_ = file->message_type(1);
  static const int server_info_offsets_[4] = {
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, server_type_),
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, server_ip_),
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, server_port_),
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, attributes_),
  };
  server_info_reflection_ =
    new ::google::protobuf::internal::GeneratedMessageReflection(
      server_info_descriptor_,
      server_info::default_instance_,
      server_info_offsets_,
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, _has_bits_[0]),
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(server_info, _unknown_fields_),
      -1,
      ::google::protobuf::DescriptorPool::generated_pool(),
      ::google::protobuf::MessageFactory::generated_factory(),
      sizeof(server_info));
  msg_packet_descriptor_ = file->message_type(2);
  static const int msg_packet_offsets_[2] = {
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(msg_packet, msgid_),
    GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(msg_packet, msginfo_),
  };
  msg_packet_reflection_ =
    new ::google::protobuf::internal::GeneratedMessageReflection(
      msg_packet_descriptor_,
      msg_packet::default_instance_,
      msg_packet_offsets_,
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(msg_packet, _has_bits_[0]),
      GOOGLE_PROTOBUF_GENERATED_MESSAGE_FIELD_OFFSET(msg_packet, _unknown_fields_),
      -1,
      ::google::protobuf::DescriptorPool::generated_pool(),
      ::google::protobuf::MessageFactory::generated_factory(),
      sizeof(msg_packet));
  e_server_type_descriptor_ = file->enum_type(0);
}

namespace {

GOOGLE_PROTOBUF_DECLARE_ONCE(protobuf_AssignDescriptors_once_);
inline void protobuf_AssignDescriptorsOnce() {
  ::google::protobuf::GoogleOnceInit(&protobuf_AssignDescriptors_once_,
                 &protobuf_AssignDesc_server_5fbase_2eproto);
}

void protobuf_RegisterTypes(const ::std::string&) {
  protobuf_AssignDescriptorsOnce();
  ::google::protobuf::MessageFactory::InternalRegisterGeneratedMessage(
    server_attributes_descriptor_, &server_attributes::default_instance());
  ::google::protobuf::MessageFactory::InternalRegisterGeneratedMessage(
    server_info_descriptor_, &server_info::default_instance());
  ::google::protobuf::MessageFactory::InternalRegisterGeneratedMessage(
    msg_packet_descriptor_, &msg_packet::default_instance());
}

}  // namespace

void protobuf_ShutdownFile_server_5fbase_2eproto() {
  delete server_attributes::default_instance_;
  delete server_attributes_reflection_;
  delete server_info::default_instance_;
  delete server_info_reflection_;
  delete msg_packet::default_instance_;
  delete msg_packet_reflection_;
}

void protobuf_AddDesc_server_5fbase_2eproto() {
  static bool already_here = false;
  if (already_here) return;
  already_here = true;
  GOOGLE_PROTOBUF_VERIFY_VERSION;

  ::google::protobuf::DescriptorPool::InternalAddGeneratedFile(
    "\n\021server_base.proto\022\020server_protocols\",\n"
    "\021server_attributes\022\027\n\014client_count\030\001 \001(\005"
    ":\0010\"\244\001\n\013server_info\0224\n\013server_type\030\001 \001(\016"
    "2\037.server_protocols.e_server_type\022\021\n\tser"
    "ver_ip\030\002 \001(\t\022\023\n\013server_port\030\003 \001(\007\0227\n\natt"
    "ributes\030\004 \001(\0132#.server_protocols.server_"
    "attributes\",\n\nmsg_packet\022\r\n\005msgid\030\001 \001(\007\022"
    "\017\n\007msginfo\030\002 \001(\014*\204\001\n\re_server_type\022\021\n\re_"
    "st_undefine\020\000\022\r\n\te_st_gate\020\001\022\016\n\ne_st_log"
    "in\020\002\022\016\n\ne_st_world\020\003\022\017\n\013e_st_center\020\004\022\016\n"
    "\ne_st_logic\020\005\022\020\n\014e_st_monitor\020\006", 431);
  ::google::protobuf::MessageFactory::InternalRegisterGeneratedFile(
    "server_base.proto", &protobuf_RegisterTypes);
  server_attributes::default_instance_ = new server_attributes();
  server_info::default_instance_ = new server_info();
  msg_packet::default_instance_ = new msg_packet();
  server_attributes::default_instance_->InitAsDefaultInstance();
  server_info::default_instance_->InitAsDefaultInstance();
  msg_packet::default_instance_->InitAsDefaultInstance();
  ::google::protobuf::internal::OnShutdown(&protobuf_ShutdownFile_server_5fbase_2eproto);
}

// Force AddDescriptors() to be called at static initialization time.
struct StaticDescriptorInitializer_server_5fbase_2eproto {
  StaticDescriptorInitializer_server_5fbase_2eproto() {
    protobuf_AddDesc_server_5fbase_2eproto();
  }
} static_descriptor_initializer_server_5fbase_2eproto_;
const ::google::protobuf::EnumDescriptor* e_server_type_descriptor() {
  protobuf_AssignDescriptorsOnce();
  return e_server_type_descriptor_;
}
bool e_server_type_IsValid(int value) {
  switch(value) {
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
    case 5:
    case 6:
      return true;
    default:
      return false;
  }
}


// ===================================================================

#ifndef _MSC_VER
const int server_attributes::kClientCountFieldNumber;
#endif  // !_MSC_VER

server_attributes::server_attributes()
  : ::google::protobuf::Message() {
  SharedCtor();
  // @@protoc_insertion_point(constructor:server_protocols.server_attributes)
}

void server_attributes::InitAsDefaultInstance() {
}

server_attributes::server_attributes(const server_attributes& from)
  : ::google::protobuf::Message() {
  SharedCtor();
  MergeFrom(from);
  // @@protoc_insertion_point(copy_constructor:server_protocols.server_attributes)
}

void server_attributes::SharedCtor() {
  _cached_size_ = 0;
  client_count_ = 0;
  ::memset(_has_bits_, 0, sizeof(_has_bits_));
}

server_attributes::~server_attributes() {
  // @@protoc_insertion_point(destructor:server_protocols.server_attributes)
  SharedDtor();
}

void server_attributes::SharedDtor() {
  if (this != default_instance_) {
  }
}

void server_attributes::SetCachedSize(int size) const {
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
}
const ::google::protobuf::Descriptor* server_attributes::descriptor() {
  protobuf_AssignDescriptorsOnce();
  return server_attributes_descriptor_;
}

const server_attributes& server_attributes::default_instance() {
  if (default_instance_ == NULL) protobuf_AddDesc_server_5fbase_2eproto();
  return *default_instance_;
}

server_attributes* server_attributes::default_instance_ = NULL;

server_attributes* server_attributes::New() const {
  return new server_attributes;
}

void server_attributes::Clear() {
  client_count_ = 0;
  ::memset(_has_bits_, 0, sizeof(_has_bits_));
  mutable_unknown_fields()->Clear();
}

bool server_attributes::MergePartialFromCodedStream(
    ::google::protobuf::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!(EXPRESSION)) goto failure
  ::google::protobuf::uint32 tag;
  // @@protoc_insertion_point(parse_start:server_protocols.server_attributes)
  for (;;) {
    ::std::pair< ::google::protobuf::uint32, bool> p = input->ReadTagWithCutoff(127);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::google::protobuf::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // optional int32 client_count = 1 [default = 0];
      case 1: {
        if (tag == 8) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::int32, ::google::protobuf::internal::WireFormatLite::TYPE_INT32>(
                 input, &client_count_)));
          set_has_client_count();
        } else {
          goto handle_unusual;
        }
        if (input->ExpectAtEnd()) goto success;
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0 ||
            ::google::protobuf::internal::WireFormatLite::GetTagWireType(tag) ==
            ::google::protobuf::internal::WireFormatLite::WIRETYPE_END_GROUP) {
          goto success;
        }
        DO_(::google::protobuf::internal::WireFormat::SkipField(
              input, tag, mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:server_protocols.server_attributes)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:server_protocols.server_attributes)
  return false;
#undef DO_
}

void server_attributes::SerializeWithCachedSizes(
    ::google::protobuf::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:server_protocols.server_attributes)
  // optional int32 client_count = 1 [default = 0];
  if (has_client_count()) {
    ::google::protobuf::internal::WireFormatLite::WriteInt32(1, this->client_count(), output);
  }

  if (!unknown_fields().empty()) {
    ::google::protobuf::internal::WireFormat::SerializeUnknownFields(
        unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:server_protocols.server_attributes)
}

::google::protobuf::uint8* server_attributes::SerializeWithCachedSizesToArray(
    ::google::protobuf::uint8* target) const {
  // @@protoc_insertion_point(serialize_to_array_start:server_protocols.server_attributes)
  // optional int32 client_count = 1 [default = 0];
  if (has_client_count()) {
    target = ::google::protobuf::internal::WireFormatLite::WriteInt32ToArray(1, this->client_count(), target);
  }

  if (!unknown_fields().empty()) {
    target = ::google::protobuf::internal::WireFormat::SerializeUnknownFieldsToArray(
        unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:server_protocols.server_attributes)
  return target;
}

int server_attributes::ByteSize() const {
  int total_size = 0;

  if (_has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    // optional int32 client_count = 1 [default = 0];
    if (has_client_count()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::Int32Size(
          this->client_count());
    }

  }
  if (!unknown_fields().empty()) {
    total_size +=
      ::google::protobuf::internal::WireFormat::ComputeUnknownFieldsSize(
        unknown_fields());
  }
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = total_size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
  return total_size;
}

void server_attributes::MergeFrom(const ::google::protobuf::Message& from) {
  GOOGLE_CHECK_NE(&from, this);
  const server_attributes* source =
    ::google::protobuf::internal::dynamic_cast_if_available<const server_attributes*>(
      &from);
  if (source == NULL) {
    ::google::protobuf::internal::ReflectionOps::Merge(from, this);
  } else {
    MergeFrom(*source);
  }
}

void server_attributes::MergeFrom(const server_attributes& from) {
  GOOGLE_CHECK_NE(&from, this);
  if (from._has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    if (from.has_client_count()) {
      set_client_count(from.client_count());
    }
  }
  mutable_unknown_fields()->MergeFrom(from.unknown_fields());
}

void server_attributes::CopyFrom(const ::google::protobuf::Message& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void server_attributes::CopyFrom(const server_attributes& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool server_attributes::IsInitialized() const {

  return true;
}

void server_attributes::Swap(server_attributes* other) {
  if (other != this) {
    std::swap(client_count_, other->client_count_);
    std::swap(_has_bits_[0], other->_has_bits_[0]);
    _unknown_fields_.Swap(&other->_unknown_fields_);
    std::swap(_cached_size_, other->_cached_size_);
  }
}

::google::protobuf::Metadata server_attributes::GetMetadata() const {
  protobuf_AssignDescriptorsOnce();
  ::google::protobuf::Metadata metadata;
  metadata.descriptor = server_attributes_descriptor_;
  metadata.reflection = server_attributes_reflection_;
  return metadata;
}


// ===================================================================

#ifndef _MSC_VER
const int server_info::kServerTypeFieldNumber;
const int server_info::kServerIpFieldNumber;
const int server_info::kServerPortFieldNumber;
const int server_info::kAttributesFieldNumber;
#endif  // !_MSC_VER

server_info::server_info()
  : ::google::protobuf::Message() {
  SharedCtor();
  // @@protoc_insertion_point(constructor:server_protocols.server_info)
}

void server_info::InitAsDefaultInstance() {
  attributes_ = const_cast< ::server_protocols::server_attributes*>(&::server_protocols::server_attributes::default_instance());
}

server_info::server_info(const server_info& from)
  : ::google::protobuf::Message() {
  SharedCtor();
  MergeFrom(from);
  // @@protoc_insertion_point(copy_constructor:server_protocols.server_info)
}

void server_info::SharedCtor() {
  ::google::protobuf::internal::GetEmptyString();
  _cached_size_ = 0;
  server_type_ = 0;
  server_ip_ = const_cast< ::std::string*>(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  server_port_ = 0u;
  attributes_ = NULL;
  ::memset(_has_bits_, 0, sizeof(_has_bits_));
}

server_info::~server_info() {
  // @@protoc_insertion_point(destructor:server_protocols.server_info)
  SharedDtor();
}

void server_info::SharedDtor() {
  if (server_ip_ != &::google::protobuf::internal::GetEmptyStringAlreadyInited()) {
    delete server_ip_;
  }
  if (this != default_instance_) {
    delete attributes_;
  }
}

void server_info::SetCachedSize(int size) const {
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
}
const ::google::protobuf::Descriptor* server_info::descriptor() {
  protobuf_AssignDescriptorsOnce();
  return server_info_descriptor_;
}

const server_info& server_info::default_instance() {
  if (default_instance_ == NULL) protobuf_AddDesc_server_5fbase_2eproto();
  return *default_instance_;
}

server_info* server_info::default_instance_ = NULL;

server_info* server_info::New() const {
  return new server_info;
}

void server_info::Clear() {
#define OFFSET_OF_FIELD_(f) (reinterpret_cast<char*>(      \
  &reinterpret_cast<server_info*>(16)->f) - \
   reinterpret_cast<char*>(16))

#define ZR_(first, last) do {                              \
    size_t f = OFFSET_OF_FIELD_(first);                    \
    size_t n = OFFSET_OF_FIELD_(last) - f + sizeof(last);  \
    ::memset(&first, 0, n);                                \
  } while (0)

  if (_has_bits_[0 / 32] & 15) {
    ZR_(server_type_, server_port_);
    if (has_server_ip()) {
      if (server_ip_ != &::google::protobuf::internal::GetEmptyStringAlreadyInited()) {
        server_ip_->clear();
      }
    }
    if (has_attributes()) {
      if (attributes_ != NULL) attributes_->::server_protocols::server_attributes::Clear();
    }
  }

#undef OFFSET_OF_FIELD_
#undef ZR_

  ::memset(_has_bits_, 0, sizeof(_has_bits_));
  mutable_unknown_fields()->Clear();
}

bool server_info::MergePartialFromCodedStream(
    ::google::protobuf::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!(EXPRESSION)) goto failure
  ::google::protobuf::uint32 tag;
  // @@protoc_insertion_point(parse_start:server_protocols.server_info)
  for (;;) {
    ::std::pair< ::google::protobuf::uint32, bool> p = input->ReadTagWithCutoff(127);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::google::protobuf::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // optional .server_protocols.e_server_type server_type = 1;
      case 1: {
        if (tag == 8) {
          int value;
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   int, ::google::protobuf::internal::WireFormatLite::TYPE_ENUM>(
                 input, &value)));
          if (::server_protocols::e_server_type_IsValid(value)) {
            set_server_type(static_cast< ::server_protocols::e_server_type >(value));
          } else {
            mutable_unknown_fields()->AddVarint(1, value);
          }
        } else {
          goto handle_unusual;
        }
        if (input->ExpectTag(18)) goto parse_server_ip;
        break;
      }

      // optional string server_ip = 2;
      case 2: {
        if (tag == 18) {
         parse_server_ip:
          DO_(::google::protobuf::internal::WireFormatLite::ReadString(
                input, this->mutable_server_ip()));
          ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
            this->server_ip().data(), this->server_ip().length(),
            ::google::protobuf::internal::WireFormat::PARSE,
            "server_ip");
        } else {
          goto handle_unusual;
        }
        if (input->ExpectTag(29)) goto parse_server_port;
        break;
      }

      // optional fixed32 server_port = 3;
      case 3: {
        if (tag == 29) {
         parse_server_port:
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::uint32, ::google::protobuf::internal::WireFormatLite::TYPE_FIXED32>(
                 input, &server_port_)));
          set_has_server_port();
        } else {
          goto handle_unusual;
        }
        if (input->ExpectTag(34)) goto parse_attributes;
        break;
      }

      // optional .server_protocols.server_attributes attributes = 4;
      case 4: {
        if (tag == 34) {
         parse_attributes:
          DO_(::google::protobuf::internal::WireFormatLite::ReadMessageNoVirtual(
               input, mutable_attributes()));
        } else {
          goto handle_unusual;
        }
        if (input->ExpectAtEnd()) goto success;
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0 ||
            ::google::protobuf::internal::WireFormatLite::GetTagWireType(tag) ==
            ::google::protobuf::internal::WireFormatLite::WIRETYPE_END_GROUP) {
          goto success;
        }
        DO_(::google::protobuf::internal::WireFormat::SkipField(
              input, tag, mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:server_protocols.server_info)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:server_protocols.server_info)
  return false;
#undef DO_
}

void server_info::SerializeWithCachedSizes(
    ::google::protobuf::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:server_protocols.server_info)
  // optional .server_protocols.e_server_type server_type = 1;
  if (has_server_type()) {
    ::google::protobuf::internal::WireFormatLite::WriteEnum(
      1, this->server_type(), output);
  }

  // optional string server_ip = 2;
  if (has_server_ip()) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->server_ip().data(), this->server_ip().length(),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "server_ip");
    ::google::protobuf::internal::WireFormatLite::WriteStringMaybeAliased(
      2, this->server_ip(), output);
  }

  // optional fixed32 server_port = 3;
  if (has_server_port()) {
    ::google::protobuf::internal::WireFormatLite::WriteFixed32(3, this->server_port(), output);
  }

  // optional .server_protocols.server_attributes attributes = 4;
  if (has_attributes()) {
    ::google::protobuf::internal::WireFormatLite::WriteMessageMaybeToArray(
      4, this->attributes(), output);
  }

  if (!unknown_fields().empty()) {
    ::google::protobuf::internal::WireFormat::SerializeUnknownFields(
        unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:server_protocols.server_info)
}

::google::protobuf::uint8* server_info::SerializeWithCachedSizesToArray(
    ::google::protobuf::uint8* target) const {
  // @@protoc_insertion_point(serialize_to_array_start:server_protocols.server_info)
  // optional .server_protocols.e_server_type server_type = 1;
  if (has_server_type()) {
    target = ::google::protobuf::internal::WireFormatLite::WriteEnumToArray(
      1, this->server_type(), target);
  }

  // optional string server_ip = 2;
  if (has_server_ip()) {
    ::google::protobuf::internal::WireFormat::VerifyUTF8StringNamedField(
      this->server_ip().data(), this->server_ip().length(),
      ::google::protobuf::internal::WireFormat::SERIALIZE,
      "server_ip");
    target =
      ::google::protobuf::internal::WireFormatLite::WriteStringToArray(
        2, this->server_ip(), target);
  }

  // optional fixed32 server_port = 3;
  if (has_server_port()) {
    target = ::google::protobuf::internal::WireFormatLite::WriteFixed32ToArray(3, this->server_port(), target);
  }

  // optional .server_protocols.server_attributes attributes = 4;
  if (has_attributes()) {
    target = ::google::protobuf::internal::WireFormatLite::
      WriteMessageNoVirtualToArray(
        4, this->attributes(), target);
  }

  if (!unknown_fields().empty()) {
    target = ::google::protobuf::internal::WireFormat::SerializeUnknownFieldsToArray(
        unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:server_protocols.server_info)
  return target;
}

int server_info::ByteSize() const {
  int total_size = 0;

  if (_has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    // optional .server_protocols.e_server_type server_type = 1;
    if (has_server_type()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::EnumSize(this->server_type());
    }

    // optional string server_ip = 2;
    if (has_server_ip()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::StringSize(
          this->server_ip());
    }

    // optional fixed32 server_port = 3;
    if (has_server_port()) {
      total_size += 1 + 4;
    }

    // optional .server_protocols.server_attributes attributes = 4;
    if (has_attributes()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::MessageSizeNoVirtual(
          this->attributes());
    }

  }
  if (!unknown_fields().empty()) {
    total_size +=
      ::google::protobuf::internal::WireFormat::ComputeUnknownFieldsSize(
        unknown_fields());
  }
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = total_size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
  return total_size;
}

void server_info::MergeFrom(const ::google::protobuf::Message& from) {
  GOOGLE_CHECK_NE(&from, this);
  const server_info* source =
    ::google::protobuf::internal::dynamic_cast_if_available<const server_info*>(
      &from);
  if (source == NULL) {
    ::google::protobuf::internal::ReflectionOps::Merge(from, this);
  } else {
    MergeFrom(*source);
  }
}

void server_info::MergeFrom(const server_info& from) {
  GOOGLE_CHECK_NE(&from, this);
  if (from._has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    if (from.has_server_type()) {
      set_server_type(from.server_type());
    }
    if (from.has_server_ip()) {
      set_server_ip(from.server_ip());
    }
    if (from.has_server_port()) {
      set_server_port(from.server_port());
    }
    if (from.has_attributes()) {
      mutable_attributes()->::server_protocols::server_attributes::MergeFrom(from.attributes());
    }
  }
  mutable_unknown_fields()->MergeFrom(from.unknown_fields());
}

void server_info::CopyFrom(const ::google::protobuf::Message& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void server_info::CopyFrom(const server_info& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool server_info::IsInitialized() const {

  return true;
}

void server_info::Swap(server_info* other) {
  if (other != this) {
    std::swap(server_type_, other->server_type_);
    std::swap(server_ip_, other->server_ip_);
    std::swap(server_port_, other->server_port_);
    std::swap(attributes_, other->attributes_);
    std::swap(_has_bits_[0], other->_has_bits_[0]);
    _unknown_fields_.Swap(&other->_unknown_fields_);
    std::swap(_cached_size_, other->_cached_size_);
  }
}

::google::protobuf::Metadata server_info::GetMetadata() const {
  protobuf_AssignDescriptorsOnce();
  ::google::protobuf::Metadata metadata;
  metadata.descriptor = server_info_descriptor_;
  metadata.reflection = server_info_reflection_;
  return metadata;
}


// ===================================================================

#ifndef _MSC_VER
const int msg_packet::kMsgidFieldNumber;
const int msg_packet::kMsginfoFieldNumber;
#endif  // !_MSC_VER

msg_packet::msg_packet()
  : ::google::protobuf::Message() {
  SharedCtor();
  // @@protoc_insertion_point(constructor:server_protocols.msg_packet)
}

void msg_packet::InitAsDefaultInstance() {
}

msg_packet::msg_packet(const msg_packet& from)
  : ::google::protobuf::Message() {
  SharedCtor();
  MergeFrom(from);
  // @@protoc_insertion_point(copy_constructor:server_protocols.msg_packet)
}

void msg_packet::SharedCtor() {
  ::google::protobuf::internal::GetEmptyString();
  _cached_size_ = 0;
  msgid_ = 0u;
  msginfo_ = const_cast< ::std::string*>(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
  ::memset(_has_bits_, 0, sizeof(_has_bits_));
}

msg_packet::~msg_packet() {
  // @@protoc_insertion_point(destructor:server_protocols.msg_packet)
  SharedDtor();
}

void msg_packet::SharedDtor() {
  if (msginfo_ != &::google::protobuf::internal::GetEmptyStringAlreadyInited()) {
    delete msginfo_;
  }
  if (this != default_instance_) {
  }
}

void msg_packet::SetCachedSize(int size) const {
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
}
const ::google::protobuf::Descriptor* msg_packet::descriptor() {
  protobuf_AssignDescriptorsOnce();
  return msg_packet_descriptor_;
}

const msg_packet& msg_packet::default_instance() {
  if (default_instance_ == NULL) protobuf_AddDesc_server_5fbase_2eproto();
  return *default_instance_;
}

msg_packet* msg_packet::default_instance_ = NULL;

msg_packet* msg_packet::New() const {
  return new msg_packet;
}

void msg_packet::Clear() {
  if (_has_bits_[0 / 32] & 3) {
    msgid_ = 0u;
    if (has_msginfo()) {
      if (msginfo_ != &::google::protobuf::internal::GetEmptyStringAlreadyInited()) {
        msginfo_->clear();
      }
    }
  }
  ::memset(_has_bits_, 0, sizeof(_has_bits_));
  mutable_unknown_fields()->Clear();
}

bool msg_packet::MergePartialFromCodedStream(
    ::google::protobuf::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!(EXPRESSION)) goto failure
  ::google::protobuf::uint32 tag;
  // @@protoc_insertion_point(parse_start:server_protocols.msg_packet)
  for (;;) {
    ::std::pair< ::google::protobuf::uint32, bool> p = input->ReadTagWithCutoff(127);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::google::protobuf::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // optional fixed32 msgid = 1;
      case 1: {
        if (tag == 13) {
          DO_((::google::protobuf::internal::WireFormatLite::ReadPrimitive<
                   ::google::protobuf::uint32, ::google::protobuf::internal::WireFormatLite::TYPE_FIXED32>(
                 input, &msgid_)));
          set_has_msgid();
        } else {
          goto handle_unusual;
        }
        if (input->ExpectTag(18)) goto parse_msginfo;
        break;
      }

      // optional bytes msginfo = 2;
      case 2: {
        if (tag == 18) {
         parse_msginfo:
          DO_(::google::protobuf::internal::WireFormatLite::ReadBytes(
                input, this->mutable_msginfo()));
        } else {
          goto handle_unusual;
        }
        if (input->ExpectAtEnd()) goto success;
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0 ||
            ::google::protobuf::internal::WireFormatLite::GetTagWireType(tag) ==
            ::google::protobuf::internal::WireFormatLite::WIRETYPE_END_GROUP) {
          goto success;
        }
        DO_(::google::protobuf::internal::WireFormat::SkipField(
              input, tag, mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:server_protocols.msg_packet)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:server_protocols.msg_packet)
  return false;
#undef DO_
}

void msg_packet::SerializeWithCachedSizes(
    ::google::protobuf::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:server_protocols.msg_packet)
  // optional fixed32 msgid = 1;
  if (has_msgid()) {
    ::google::protobuf::internal::WireFormatLite::WriteFixed32(1, this->msgid(), output);
  }

  // optional bytes msginfo = 2;
  if (has_msginfo()) {
    ::google::protobuf::internal::WireFormatLite::WriteBytesMaybeAliased(
      2, this->msginfo(), output);
  }

  if (!unknown_fields().empty()) {
    ::google::protobuf::internal::WireFormat::SerializeUnknownFields(
        unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:server_protocols.msg_packet)
}

::google::protobuf::uint8* msg_packet::SerializeWithCachedSizesToArray(
    ::google::protobuf::uint8* target) const {
  // @@protoc_insertion_point(serialize_to_array_start:server_protocols.msg_packet)
  // optional fixed32 msgid = 1;
  if (has_msgid()) {
    target = ::google::protobuf::internal::WireFormatLite::WriteFixed32ToArray(1, this->msgid(), target);
  }

  // optional bytes msginfo = 2;
  if (has_msginfo()) {
    target =
      ::google::protobuf::internal::WireFormatLite::WriteBytesToArray(
        2, this->msginfo(), target);
  }

  if (!unknown_fields().empty()) {
    target = ::google::protobuf::internal::WireFormat::SerializeUnknownFieldsToArray(
        unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:server_protocols.msg_packet)
  return target;
}

int msg_packet::ByteSize() const {
  int total_size = 0;

  if (_has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    // optional fixed32 msgid = 1;
    if (has_msgid()) {
      total_size += 1 + 4;
    }

    // optional bytes msginfo = 2;
    if (has_msginfo()) {
      total_size += 1 +
        ::google::protobuf::internal::WireFormatLite::BytesSize(
          this->msginfo());
    }

  }
  if (!unknown_fields().empty()) {
    total_size +=
      ::google::protobuf::internal::WireFormat::ComputeUnknownFieldsSize(
        unknown_fields());
  }
  GOOGLE_SAFE_CONCURRENT_WRITES_BEGIN();
  _cached_size_ = total_size;
  GOOGLE_SAFE_CONCURRENT_WRITES_END();
  return total_size;
}

void msg_packet::MergeFrom(const ::google::protobuf::Message& from) {
  GOOGLE_CHECK_NE(&from, this);
  const msg_packet* source =
    ::google::protobuf::internal::dynamic_cast_if_available<const msg_packet*>(
      &from);
  if (source == NULL) {
    ::google::protobuf::internal::ReflectionOps::Merge(from, this);
  } else {
    MergeFrom(*source);
  }
}

void msg_packet::MergeFrom(const msg_packet& from) {
  GOOGLE_CHECK_NE(&from, this);
  if (from._has_bits_[0 / 32] & (0xffu << (0 % 32))) {
    if (from.has_msgid()) {
      set_msgid(from.msgid());
    }
    if (from.has_msginfo()) {
      set_msginfo(from.msginfo());
    }
  }
  mutable_unknown_fields()->MergeFrom(from.unknown_fields());
}

void msg_packet::CopyFrom(const ::google::protobuf::Message& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void msg_packet::CopyFrom(const msg_packet& from) {
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool msg_packet::IsInitialized() const {

  return true;
}

void msg_packet::Swap(msg_packet* other) {
  if (other != this) {
    std::swap(msgid_, other->msgid_);
    std::swap(msginfo_, other->msginfo_);
    std::swap(_has_bits_[0], other->_has_bits_[0]);
    _unknown_fields_.Swap(&other->_unknown_fields_);
    std::swap(_cached_size_, other->_cached_size_);
  }
}

::google::protobuf::Metadata msg_packet::GetMetadata() const {
  protobuf_AssignDescriptorsOnce();
  ::google::protobuf::Metadata metadata;
  metadata.descriptor = msg_packet_descriptor_;
  metadata.reflection = msg_packet_reflection_;
  return metadata;
}


// @@protoc_insertion_point(namespace_scope)

}  // namespace server_protocols

// @@protoc_insertion_point(global_scope)
