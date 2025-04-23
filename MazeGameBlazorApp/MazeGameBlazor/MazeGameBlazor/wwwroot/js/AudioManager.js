window.AudioManager = {
    context: new (window.AudioContext || window.webkitAudioContext)(),
    gainNode: null,
    bgMusic: null,
    bgMusicBuffer: null,
    bgMusicStartTime: 0,
    bgMusicPausedAt: 0,
    currentTrackIndex: 0,
    playlist: [
        "bright-electronic-loop-251871.mp3",
        "cool-hip-hop-loop-275527.mp3",
        "epic-hybrid-logo-157092.mp3"
    ],

    init() {
        this.gainNode = this.context.createGain();
        this.gainNode.connect(this.context.destination);
        console.log("[AudioManager] Initialized");
    },

    async playEffect(fileName) {
        try {
            const response = await fetch(`/assets/audio/${fileName}`);
            const buffer = await response.arrayBuffer();
            const audio = await this.context.decodeAudioData(buffer);
            const source = this.context.createBufferSource();
            source.buffer = audio;
            source.connect(this.gainNode);
            source.start(0);
        } catch (err) {
            console.error("[AudioManager] Failed to play sound:", fileName, err);
        }
    },

    async playBackgroundMusic(track = null, volume = 0.4) {
        this.stopBackgroundMusic();

        try {
            const file = track || this.playlist[this.currentTrackIndex];
            const response = await fetch(`/assets/audio/${file}`);
            const buffer = await response.arrayBuffer();
            this.bgMusicBuffer = await this.context.decodeAudioData(buffer);
            this.bgMusicPausedAt = 0;
            this._startBufferSource(0);
            this.setVolume(volume);
        } catch (err) {
            console.error("[AudioManager] Background music failed:", err);
        }
    },

    _startBufferSource(offsetSeconds) {
        const source = this.context.createBufferSource();
        source.buffer = this.bgMusicBuffer;
        source.loop = true;
        source.connect(this.gainNode);
        source.start(0, offsetSeconds);
        this.bgMusic = source;
        this.bgMusicStartTime = this.context.currentTime - offsetSeconds;
    },

    pauseBackgroundMusic() {
        if (this.bgMusic && this.bgMusicBuffer) {
            this.bgMusic.stop();
            this.bgMusicPausedAt = this.context.currentTime - this.bgMusicStartTime;
            this.bgMusic = null;
        }
    },

    resumeBackgroundMusic() {
        if (!this.bgMusic && this.bgMusicBuffer) {
            this._startBufferSource(this.bgMusicPausedAt || 0);
        }
    },

    stopBackgroundMusic() {
        if (this.bgMusic) {
            this.bgMusic.stop();
            this.bgMusic = null;
            this.bgMusicBuffer = null;
            this.bgMusicStartTime = 0;
            this.bgMusicPausedAt = 0;
        }
    },

    nextTrack() {
        this.currentTrackIndex = (this.currentTrackIndex + 1) % this.playlist.length;
        this.playBackgroundMusic();
    },

    setVolume(volume) {
        if (this.gainNode) {
            this.gainNode.gain.value = volume;
        }
    }
};
