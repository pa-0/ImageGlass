
export default class TabImage {
  /**
   * Loads settings for tab Image.
   */
  static loadSettings() {
    const colorProfile = (_pageSettings.config.ColorProfile as string || '');
    if (colorProfile.includes('.')) {
      query<HTMLSelectElement>('[name="ColorProfile"]').value = 'Custom';
      query('#Lnk_CustomColorProfile').innerText = colorProfile;
    }

    this.handleColorProfileChanged();
    this.handleUseEmbeddedThumbnailOptionsChanged();
  }


  /**
   * Add events for tab Image.
   */
  static addEvents() {
    query('#Btn_BrowseColorProfile').addEventListener('click', async () => {
      const profileFilePath = await postAsync<string>('Btn_BrowseColorProfile');
      query('#Lnk_CustomColorProfile').innerText = profileFilePath;
    }, false);
  
    query('#Lnk_CustomColorProfile').addEventListener('click', () => {
      const profileFilePath = query('#Lnk_CustomColorProfile').innerText.trim();
      post('Lnk_CustomColorProfile', profileFilePath);
    }, false);
  
    query('[name="ColorProfile"]').addEventListener('change', () => this.handleColorProfileChanged(), false);
  
    query('[name="UseEmbeddedThumbnailRawFormats"]').addEventListener('input', () => this.handleUseEmbeddedThumbnailOptionsChanged(), false);
    query('[name="UseEmbeddedThumbnailOtherFormats"]').addEventListener('input', () => this.handleUseEmbeddedThumbnailOptionsChanged(), false);
  }


  static handleColorProfileChanged() {
    const selectEl = query<HTMLSelectElement>('[name="ColorProfile"]');
    const useCustomProfile = selectEl.value === 'Custom';
  
    query('#Btn_BrowseColorProfile').hidden = !useCustomProfile;
    query('#Section_CustomColorProfile').hidden = !useCustomProfile;
  }


  static handleUseEmbeddedThumbnailOptionsChanged() {
    const enableForRaw = query<HTMLInputElement>('[name="UseEmbeddedThumbnailRawFormats"]').checked;
    const enableForOthers = query<HTMLInputElement>('[name="UseEmbeddedThumbnailOtherFormats"]').checked;
    const showSizeSection = enableForRaw || enableForOthers;
  
    query('#Section_EmbeddedThumbnailSize').hidden = !showSizeSection;
  }
}