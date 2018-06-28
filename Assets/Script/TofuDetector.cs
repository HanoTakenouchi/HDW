//using UnityEngine;
//using TMPro;


///// <summary>
///// 豆腐を検出するコンポーネント
///// </summary>
//[ExecuteInEditMode]
//public class TofuDetector : MonoBehaviour
//{
//    void Awake()
//    {
//        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
//    }

//    private void OnDestroy()
//    {
//        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
//    }

//    private void ON_TEXT_CHANGED(Object obj)
//    {
//        if (obj is TextMeshProUGUI)
//        {
//            var text = (TextMeshProUGUI)obj;

//            foreach (var chara in text.text)
//            {
//                TMP_Glyph glyph;
//                TMP_FontUtilities.SearchForGlyph(text.font, chara, out glyph);

//                if (glyph == null)
//                {
//                    Debug.Log("Detection Tofu!! " + chara);
//                    // ここでtxt等にログを残したり、slackAPIを叩いたり等
//                }
//            }
//        }
//    }
//}
