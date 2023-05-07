using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using MongoDB.Bson;
public partial class User : IRealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId? Id { get; set; }
    [MapTo("addr")]
    public string? Addr { get; set; }
    [MapTo("password")]
    public string? Password { get; set; }
    [MapTo("username")]
    public string? Username { get; set; }
}