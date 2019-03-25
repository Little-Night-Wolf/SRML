﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data
{
    public class CompoundDataPiece : DataPiece
    {
        public CompoundDataPiece(string key)
        {
            this.typeId = DataType.COMPOUND;
            this.data = new HashSet<DataPiece>();
            this.key = key;
        }

        internal CompoundDataPiece() { }

        public HashSet<DataPiece> dataList
        {
            get { return data as HashSet<DataPiece>; }
        }

        public DataPiece this[string index]
        {
            get { return dataList.First((x) => x.key == index); }
        }

        public object GetValue(string key)
        {
            return this[key].GetValue();
        }

        public T GetValue<T>(string key)
        {
            return (T)GetValue(key);
        }

        public DataPiece AddPiece(DataPiece piece)
        {
            dataList.Add(piece);
            return piece;
        }

        public bool HasPiece(string key)
        {
            return dataList.Any((x) => x.key == key);
        }

        public DataPiece GetPiece(string key,Type type)
        {
            if (HasPiece(key)) return this[key];
            return AddPiece(new DataPiece(key, type));
        }

        public DataPiece SetPiece(string key, object val)
        {
            var p = GetPiece(key, val.GetType());
            p.SetValue(val);

            return p;
        }

        public CompoundDataPiece GetCompoundPiece(string key)
        {
            //Debug.Log($"Trying to get compound piece {key} on {this.key}");
            if (dataList.FirstOrDefault((x) => x.key == key) is DataPiece piece)
            {
                if (!(piece is CompoundDataPiece p)) throw new Exception("Piece is not compound data piece");
                return p;
            }

            return AddPiece(new CompoundDataPiece(key)) as CompoundDataPiece;
        }

        public DataPiece GetPiece<T>(string key)
        {
            return GetPiece(key, typeof(T));
        }


        public void SetValue(string key, object value)
        {
            this[key].SetValue(value);
        }

        public void Stringify(StringBuilder builder, Action<String> adder=null)
        {
            
            if (adder == null)
            {
                adder = (x) => builder.AppendLine(x);
            }

            adder($"COMPOUND {key} = ");
            Action<String> newadder = (x) => adder("    " + x);
            foreach (var v in dataList)
            {
                if (v is CompoundDataPiece piece)
                {
                    piece.Stringify(builder, newadder);
                }
                else
                {
                    newadder(v.ToString());
                }
            }
        }
    }
}