# SoNgCaMp

![가로1080_8](https://github.com/user-attachments/assets/4aa0fc26-c548-4995-86b3-895efaf5817f)


## 🧑‍🎤 一言紹介
音楽家とリスナーが集まるメタバースプラットフォーム。
[韓国電波振興協会] 第2期メタバースアカデミー最終成果発表会 奨励賞受賞作品
<br>
<br>

## 📺 プレイ動画
[https://www.youtube.com/shorts/xhghoX2gcs4](https://youtu.be/avcJFLEoey4?si=_okbCBRPufecBxak)
<br>
<br>



## 🦦 アピールポイントと挑戦課題

<details>
<summary>① プレイヤーのカスタマイズ情報をサーバーに保存</summary>
![가로1080](https://github.com/user-attachments/assets/101ea34a-b66c-4322-8422-a41291c58fb5)




### 🔧 実装概要

アクセサリーの着用状況や各パーツの色調整などのカスタマイズ情報をサーバーに送信しました。

### 💻 ソースコード

```csharp
using UnityEngine.Networking;
using static HttpController;

HttpInfo httpInfo = new HttpInfo();

LoginResponseDTO dto = (LoginResponseDTO)PlayerManager.Get.GetValue("LoginInfo");

UserInfo_customizing userInfo_Customizing = new UserInfo_customizing(
    characterInfo.characterType,
    characterInfo.hexString_cloth,
    characterInfo.hexString_face,
    characterInfo.hexString_ribbon,
    characterInfo.hexString_skin,
    characterInfo.isBagON,
    characterInfo.isCapON,
    characterInfo.isCrownON,
    characterInfo.isGlassON,
    dto.userNo
);

dto.characterType = characterInfo.characterType;
dto.hexStringCloth = characterInfo.hexString_cloth;
dto.hexStringFace = characterInfo.hexString_face;
dto.hexStringRibbon = characterInfo.hexString_ribbon;
dto.hexStringSkin = characterInfo.hexString_skin;
dto.isBagOn = characterInfo.isBagON;
dto.isCapOn = characterInfo.isCapON;
dto.isCrownOn = characterInfo.isCrownON;
dto.isGlassOn = characterInfo.isGlassON;

PlayerManager.Get.Add("LoginInfo", dto);

// カスタマイズ保存
httpInfo.Set(RequestType.POST, "api/v1/users/customize", (DownloadHandler downHandler) =>
{
    // 저장하면 로비로 이동
    print(downHandler.text);

    // 요청하고 한번 더 불러오고
    ConnectionManager.Get.onJoinRoom = () =>
    {
        PhotonNetwork.LoadLevel(4);
    };
    ConnectionManager.Get.ConnectToPhoton();

}, true);

httpInfo.body = JsonUtility.ToJson(userInfo_Customizing);
HttpManager.Get().SendRequest(httpInfo);

```

</details>

<details>
<summary>② MIDIファイルを分析し、ゲームタイルを生成</summary>
    

### 🔧 実装概要
楽曲のMIDIファイルを分析し、音の長さやメロディーの種類に基づいて、Short、Long、Dragのゲームタイルを生成しました。

### 💻 ソースコード
```csharp
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

pulic class Midi2Note : MonoBehaviour
{
		MidiWequencer midiSequencer;
		private StreamSynthesizer midiStreamSynthesizer;
		public bankFilePath = "GM Bank/gm";
		public int bufferSize = 1024;
		private float[] sampleBuffer;
		public string midiPath = "ichumonandodemo.mid.txt";
		
		private void Awake()
		{
				midiStreamSynthesizer = new StreamSynthesizer(44100,2, bufferSize, 40);
				sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
				midiStreamSynthesizer.LoadBank(bankFilePath);
				midiSequencer = new MidiSequencer(midiStreamsynthesizer);
				midiSequencer.LoadMidi(midiPath, false);
		}
		
		void Start()
		{
				List<MidiEventInfo> midiEvent_selectedTrack = midiSequencer.midiAllNoteEventsDic[instrumentIdx];
				
				for(int i = 0; i < midiEvents_selectedTrack.Count; i++)
				{
						// 모든 음 순회하며 미리 노트 정보 생성해두기
				}
		}
}
```
</details>
<details>
<summary>③ タイルタイプ別のスコア判定</summary>
![가로1080_1](https://github.com/user-attachments/assets/d0fc64da-4e71-4c06-af51-8f6dc216734a)
![가로1080_2](https://github.com/user-attachments/assets/fa429318-29f0-4fd1-a24d-4bb5442cbc03)
![가로1080_3](https://github.com/user-attachments/assets/35d14a2f-6ed8-4a71-87a2-55212a25299c)



### 🔧 実装概要
タイルタイプごとにタッチ判定を行い、スコア（Miss / Bad / Good / Great / Excellent）を決定しました。Missの場合は、カメラが揺れるエフェクトを実装しました。


</details>

<br>
<br>

