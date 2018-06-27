using UnityEngine;
using TMPro;


/// <summary>
/// 文字送りアニメーション
/// サンプル「VertexJitter」を作り変えたものになります。
/// 「TypefaceAnimator」というuGUIの素晴らしいアセットのアニメーション仕様を参考にさせていただきました。
/// </summary>
public class TextAnimator : MonoBehaviour
{
    /// <summary>
    /// アニメーションプログレス(エディタ確認用)
    /// </summary>
    [SerializeField, Range(0.0f, 1.0f)]
    private float progress = 0.0f;

    /// <summary>
    /// Enable時に再生するかどうか
    /// </summary>
    [SerializeField]
    private bool playOnEnable = false;

    /// <summary>
    /// アニメーション中かどうか
    /// </summary>
    public bool isAnimating { get { return time < maxTime; } }

    /// <summary>
    /// 文字送りアニメーションデータ
    /// </summary>
    [SerializeField]
    public new TextAnimation animation;


    /// <summary>TextMeshPro Textコンポーネント</summary>
    private TMP_Text textComponent;
    /// <summary>textComponent.textInfoのキャッシュ</summary>
    private TMP_TextInfo textInfo;
    /// <summary>textComponent.textに変更があったかどうか</summary>
    private bool hasTextChanged;
    /// <summary>Start()がコールされたかどうか</summary>
    private bool isStarted;
    /// <summary>アニメーション時間</summary>
    private float time;
    /// <summary>アニメーション最大時間</summary>
    private float maxTime;


#if UNITY_EDITOR
    /// <summary>
    /// Unity Event OnValidate
    /// </summary>
    private void OnValidate()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        time = maxTime * progress;
        UpdateText(true);
        UpdateAnimation();
    }
#endif

    /// <summary>
    /// Unity Event Awake
    /// </summary>
    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        ON_TEXT_CHANGED(textComponent);
    }

    /// <summary>
    /// Unity Event OnEnable
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);

        if (playOnEnable && isStarted)
        {
            Play();
        }
    }

    /// <summary>
    /// Unity Event OnDisable
    /// </summary>
    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);

        Finish();
    }

    /// <summary>
    /// Unity Event Start
    /// </summary>
    private void Start()
    {
        isStarted = true;

        if (playOnEnable)
        {
            Play();
        }
    }

    /// <summary>
    /// Unity Event Update
    /// </summary>
    private void Update()
    {
        if (null == animation) { return; }
        if (null == textInfo) { return; }

        if (maxTime <= time) { return; }
        if (maxTime <= 0.0f) { return; }

        time += Time.deltaTime;
        if (maxTime <= time)
        {
            time = maxTime;
        }

#if UNITY_EDITOR
        progress = time / maxTime;
#endif

        UpdateAnimation();
    }

    /// <summary>
    /// TextMesh Proのtext変更時に呼び出されるメソッドです
    /// OnEnableとOnDisableにてTMPro_EventManagerに登録しています
    /// </summary>
    /// <param name="obj"></param>
    private void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == textComponent)
            hasTextChanged = true;
    }

    /// <summary>
    /// アニメーションデータの上書き設定
    /// </summary>
    public void SetAnimation(TextAnimation animation)
    {
        this.animation = animation;
    }

    /// <summary>
    /// 再生
    /// </summary>
    public void Play()
    {
        time = 0.0f;
        UpdateText(true);
        UpdateAnimation();
    }

    /// <summary>
    /// 強制終了
    /// </summary>
    public void Finish()
    {
        time = maxTime;
        UpdateAnimation();
    }

    /// <summary>
    /// TMPro Textの情報更新
    /// </summary>
    private void UpdateText(bool forceUpdate)
    {
        if (hasTextChanged || forceUpdate)
        {
            hasTextChanged = false;

            textComponent.ForceMeshUpdate(true);
            textInfo = textComponent.textInfo;

            // 各アニメーション要素で、一番時間がかかるものを最大時間として計算
            maxTime = Mathf.Max(
                CalcAnimationTotalTime(textInfo.characterCount, animation.position),
                CalcAnimationTotalTime(textInfo.characterCount, animation.rotation),
                CalcAnimationTotalTime(textInfo.characterCount, animation.scale),
                CalcAnimationTotalTime(textInfo.characterCount, animation.alpha),
                CalcAnimationTotalTime(textInfo.characterCount, animation.color)
            );
        }
    }

    /// <summary>
    /// TMPro Textの頂点情報の編集
    /// </summary>
    private void UpdateAnimation()
    {
        UpdateText(false);

        // マーカーや下線等の追加描画物はどうしても描画されてしまうので、
        // 表示最大数も合わせてアニメーションすることで対応しています。
        // 恐らくtextInfo.meshInfoのどこかにありますが、未調査の為この実装になります。
        if (animation.useMaxVisibleCharacter)
        {
            var maxVisibleCharacters = CalcAnimationCharacterCount(time, animation.alpha);
            if (textComponent.maxVisibleCharacters != maxVisibleCharacters)
            {
                textComponent.maxVisibleCharacters = maxVisibleCharacters;
                textComponent.ForceMeshUpdate();
                textInfo = textComponent.textInfo;
            }
        }

        // 開始時等MeshInfoの生成が遅れるケースがあったため小さい数値をforに使用
        var count = Mathf.Min(textInfo.characterCount, textInfo.characterInfo.Length);
        for (int i = 0; i < count; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Determine the center point of each character at the baseline.
            //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
            // Determine the center point of each character.
            //Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            // This is needed so the matrix TRS is applied at the origin for each character.
            //Vector3 offset = charMidBasline;

            if (animation.position.use || animation.rotation.use || animation.scale.use)
            {
                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;
                var vertex0 = verts[vertexIndex];
                var vertex1 = verts[vertexIndex + 1];
                var vertex2 = verts[vertexIndex + 2];
                var vertex3 = verts[vertexIndex + 3];

                if (animation.position.use)
                {
                    var ratio = animation.position.curve.Evaluate(CalcAnimationTime(time, i, animation.position));
                    var delta = Vector3.LerpUnclamped(animation.position.from, animation.position.to, ratio);
                    vertex0 += delta;
                    vertex1 += delta;
                    vertex2 += delta;
                    vertex3 += delta;
                }

                if (animation.rotation.use)
                {
                    var ratio = animation.rotation.curve.Evaluate(CalcAnimationTime(time, i, animation.rotation));
                    var delta = Vector3.LerpUnclamped(animation.rotation.from, animation.rotation.to, ratio);
                    var center = Vector3.Scale(vertex2 - vertex0, animation.pivot) + vertex0;
                    var matrix = Matrix4x4.Rotate(Quaternion.Euler(delta));
                    vertex0 = matrix.MultiplyPoint(vertex0 - center) + center;
                    vertex1 = matrix.MultiplyPoint(vertex1 - center) + center;
                    vertex2 = matrix.MultiplyPoint(vertex2 - center) + center;
                    vertex3 = matrix.MultiplyPoint(vertex3 - center) + center;
                }

                if (animation.scale.use)
                {
                    var ratio = animation.scale.curve.Evaluate(CalcAnimationTime(time, i, animation.scale));
                    var delta = Vector3.LerpUnclamped(animation.scale.from, animation.scale.to, ratio);
                    var center = Vector3.Scale(vertex2 - vertex0, animation.pivot) + vertex0;
                    vertex0 = Vector3.Scale(vertex0 - center, delta) + center;
                    vertex1 = Vector3.Scale(vertex1 - center, delta) + center;
                    vertex2 = Vector3.Scale(vertex2 - center, delta) + center;
                    vertex3 = Vector3.Scale(vertex3 - center, delta) + center;
                }

                verts[vertexIndex] = vertex0;
                verts[vertexIndex + 1] = vertex1;
                verts[vertexIndex + 2] = vertex2;
                verts[vertexIndex + 3] = vertex3;
            }

            if (animation.color.use || animation.alpha.use)
            {
                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;
                var color0 = colors[vertexIndex];
                var color1 = colors[vertexIndex + 1];
                var color2 = colors[vertexIndex + 2];
                var color3 = colors[vertexIndex + 3];

                if (animation.color.use)
                {
                    var ratio = animation.color.curve.Evaluate(CalcAnimationTime(time, i, animation.color));
                    color0 = animation.color.gradient.Evaluate(ratio);
                    color1 = color2 = color3 = color0;
                }

                if (animation.alpha.use)
                {
                    var ratio = animation.alpha.curve.Evaluate(CalcAnimationTime(time, i, animation.alpha));
                    float alpha = Mathf.Lerp(animation.alpha.from, animation.alpha.to, ratio);
                    color0.a = (byte)(color0.a * Mathf.Clamp01(alpha));
                    color1 = color2 = color3 = color0;
                }

                colors[vertexIndex] = color0;
                colors[vertexIndex + 1] = color1;
                colors[vertexIndex + 2] = color2;
                colors[vertexIndex + 3] = color3;
            }
        }

        // 表示しているマテリアルの数だけ頂点を更新します
        // <Material>や<Font>でロードした情報はmeshInfoにキャッシュされていることに注意が必要です
        for (int i = 0; i < textInfo.materialCount; i++)
        {
#if UNITY_EDITOR
            // OnValidateにてMeshの生成が遅れるケースがあったためNullチェック
            if (textInfo.meshInfo[i].mesh == null) { continue; }
#endif
            // Push changes into meshes
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    /// <summary>
    /// 文字送りアニメーションデータの要素の最大時間を算出
    /// </summary>
    static private float CalcAnimationTotalTime<TType>(int characterCount, TextAnimation.ItemBase<TType> item)
    {
        if (characterCount <= 0) { return 0.0f; }
        if (!item.use) { return 0.0f; }
        return item.delay + (characterCount - 1) * item.wave + item.time;
    }

    /// <summary>
    /// 文字送りアニメーションデータの要素の特定文字の相対時間を算出
    /// </summary>
    static private float CalcAnimationTime<TType>(float time, int characterIndex, TextAnimation.ItemBase<TType> item)
    {
        if (time < item.delay) { return 0.0f; }
        if (item.time <= 0.0f) { return 1.0f; }
        return Mathf.Clamp01(((time - item.delay) - (characterIndex * item.wave)) / item.time);
    }

    /// <summary>
    /// 文字送りアニメーションデータの要素と絶対時間から何文字目までWaveしているか算出
    /// </summary>
    static private int CalcAnimationCharacterCount<TType>(float time, TextAnimation.ItemBase<TType> item)
    {
        if (item.wave <= 0.0f) { return int.MaxValue; }
        return (int)((time - item.delay) / item.wave);
    }
}

