using System;
using System.Collections.Generic;
using UnityEngine;

public static class IDManager
{
    public static HashSet<UInt32> IDList;

    public static UInt32 AssignId() => UniqueID();

    private static UInt32 UniqueID()
    {
        if(IDList.Count == 0)
        {
            return RandomID();
        }

        while(true)
        {   
            UInt32 tempID = RandomID();
            bool unique = true;

            for(int i = 0; i < IDList.Count; i++)
            {
                if(IDList.Contains(tempID)) unique = false;
            }

            if(unique) return tempID;
        }
    }

    public static void RemoveId(UInt32 ID)
    {
        if(IDList.Contains(ID)) IDList.Remove(ID);
    }

    private static UInt32 RandomID() => (UInt32)UnityEngine.Random.Range(0, (float)UInt32.MaxValue);
}
