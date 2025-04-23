window.AudioManager = {
    context: new (window.AudioContext || window.webkitAudioContext)(),
    gainNode: null,
    bgMusic: null,
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

    playBackgroundMusic(track = null, volume = 0.4) {
        if (this.bgMusic) {
            this.bgMusic.pause();
            this.bgMusic = null;
        }

        const file = track || this.playlist[this.currentTrackIndex];
        this.bgMusic = new Audio(`/assets/audio/${file}`);
        this.bgMusic.loop = true;
        this.bgMusic.volume = volume;
        this.bgMusic.play().catch(err => {
            console.warn("[AudioManager] Autoplay blocked or error:", err);
        });
    },

    pauseBackgroundMusic() {
        if (this.bgMusic) this.bgMusic.pause();
    },

    resumeBackgroundMusic() {
        if (this.bgMusic) this.bgMusic.play();
    },

    nextTrack() {
        this.currentTrackIndex = (this.currentTrackIndex + 1) % this.playlist.length;
        this.playBackgroundMusic();
    },

    setVolume(volume) {
        if (this.bgMusic) this.bgMusic.volume = volume;
    }
};