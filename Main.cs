using System.Windows.Forms;
using DxLibDLL;

public sealed class Keyboard {
  private static Keyboard m_instance = new Keyboard();
  private static int[] m_keyPressingCount;
  private static int[] m_keyReleasingCount;
  private static readonly int KEY_NUM = 256;
  
  static Keyboard() {
    m_keyPressingCount = new int[KEY_NUM];
    m_keyReleasingCount = new int[KEY_NUM];
    for (int i = 0; i < KEY_NUM; i++){
      m_keyPressingCount[i] = m_keyReleasingCount[i] = 0;
    }
  }
  public static bool Update(bool drawKeyCodeFlag) {
    byte[] nowKeyStatus;
    nowKeyStatus = new byte[KEY_NUM];
    int counting = 0;
    DX.GetHitKeyStateAll(nowKeyStatus);
    for (int i = 0; i < KEY_NUM; i++) {
      if (nowKeyStatus[i] != 0) {
        if (m_keyReleasingCount[i] > 0) {
          m_keyReleasingCount[i] = 0;
        }
        m_keyPressingCount[i]++;
        if (drawKeyCodeFlag) {
          DX.DrawString(0, 16 * counting, "KEY:" + i, 0xffffff);
          counting++;
        }
      }
      else {
        if (m_keyPressingCount[i] > 0) {
          m_keyPressingCount[i] = 0;
        }
        m_keyReleasingCount[i]++;
      }
    }
    return true;
  }
  public static int GetPressingCount(int keyCode) {
    if (!IsAvailableCode(keyCode)) {
      return -1;
    }
    return m_keyPressingCount[keyCode];
  }

  public static int GetReleasingCount(int keyCode) {
    if (!IsAvailableCode(keyCode)) {
      return -1;
    }
    return m_keyReleasingCount[keyCode];
  }

  private static bool IsAvailableCode(int keyCode) {
    if (!(0 <= keyCode && keyCode < KEY_NUM)) {
      return false;
    }
    return true;
  }
}

class Project {
  static readonly int WIDTH = 640, HEIGHT = 480;
  static bool ProcessLoop() {
    return (DX.ScreenFlip() == 0 && DX.ProcessMessage() == 0 && DX.ClearDrawScreen() == 0 && Keyboard.Update(false));
  }
  static bool Init(){
    DX.SetOutApplicationLogValidFlag(DX.FALSE);
    DX.SetGraphMode(WIDTH, HEIGHT, 32);
    DX.SetMainWindowText("DxLib");
    DX.SetAlwaysRunFlag(DX.TRUE);
    DX.SetWindowStyleMode(7);
    DX.ChangeWindowMode(DX.TRUE);
    DX.SetWindowSizeChangeEnableFlag(DX.TRUE);
    if (DX.DxLib_Init() != 0) {
      MessageBox.Show("ウィンドウの生成に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return true;
    }
    if (DX.SetDrawScreen(DX.DX_SCREEN_BACK) != 0) {
      MessageBox.Show("ウィンドウの設定に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
      DX.DxLib_End();
      return true;
    }
    return false;
  }
  static bool Fina(){
    DX.DxLib_End();
    return false;
  }
  static void Main(string[] args){
    if (Init()) return;
    int x = 0, y = 0;
    while(ProcessLoop()){
      if (Keyboard.GetPressingCount(DX.KEY_INPUT_ESCAPE) == 1) break;
      if (Keyboard.GetPressingCount(DX.KEY_INPUT_RIGHT) > 0) x += 10;
      if (Keyboard.GetPressingCount(DX.KEY_INPUT_DOWN)  > 0) y += 10;
      if (Keyboard.GetPressingCount(DX.KEY_INPUT_LEFT)  > 0) x -= 10;
      if (Keyboard.GetPressingCount(DX.KEY_INPUT_UP)    > 0) y -= 10;
      DX.DrawString(0, 0, "FPS : " + DX.GetFPS(), DX.GetColor(0xff, 0xff, 0xff));
      DX.DrawString(0, 16, "x : " + x + "  y : " + y, DX.GetColor(0xff, 0xff, 0xff));
      DX.DrawCircle(x, y, 50, 0xffffff);
    }
    Fina();
  }
}