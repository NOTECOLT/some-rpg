using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable {
    IDeserializable Serialize();
}

public interface IDeserializable {
    ISerializable Deserialize();
}
