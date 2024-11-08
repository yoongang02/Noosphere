using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

class EventStructure
{
    public string event_id;
    public string description;
    public string repeat_Type;
    public string condition_Type;
    public string condition1;
    public string conditon2;
    public string condition3;
    public string result_id_1;
    public string result_id_2;
    public string result_id_3;
    public string evidence_id;
    public string lock_condition_id;
    public string location_id;
    public string next_Event_id;
}
public class CSVParserYKM : MonoBehaviour
{
    public Dictionary<string, T> Parse<T>(string _CSVFileName) where T : new()
    {
        //딕셔너리 생성하기
        Dictionary<string, T> dictionary = new Dictionary<string, T>();
        
        //CSV 데이터 가져오기
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        if (csvData == null)
        {
            Debug.Log(_CSVFileName + " 이름의 csv file을 찾을 수 없음.");
            return dictionary;
        }
        
        //엔터를 기준으로 줄 나누기
        string[] datas = csvData.text.Split('\n');
        //헤더 값 저장하기
        string[] headers = datas[5].Split(',');
        //7번째 줄부터 읽어오기(1~5번째 줄은 설명, 6번째 줄은 헤더)
        for (int i = 6; i < datas.Length; i++)
        {
            //빈 줄이라면 다음 줄로 넘어가기
            if(string.IsNullOrWhiteSpace(datas[i])) continue;
            //쉼표를 기준으로 분리하기
            string[] values = datas[i].Split(',');
            //첫번째 값은 키 값으로 사용
            string key = values[0];
            //제너릭 객체 생성
            T entry = new T();
            
            //나머지 값들을 T class 내의 필드들에 저장하기
            for (int j = 1; j < headers.Length && j < values.Length; j++)
            {
                FieldInfo field = typeof(T).GetField(headers[j], BindingFlags.Public);

                if (field != null)
                {
                    field.SetValue(entry,values[j]);
                }
            }
            
            //딕셔너리에 추가하기
            dictionary[key] = entry;
        }
        return dictionary;
    }

    void Start()
    {
        foreach (var line in Parse<EventStructure>("Noosphere Data Table - Event"))
        {
            Debug.Log(line.Key);
        }
    }
}
