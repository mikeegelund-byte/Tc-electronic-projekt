/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Color;

public class Constants {
    static final Color darkblue;
    static final Color grisvert;
    static final byte[] CC0;
    static final byte[] CC60;
    static final byte[] CC7F;
    static final byte[] PC1;
    static final byte[] PC2;
    static final String[] MAPPARAM;
    static final String[] DRV_EXP;
    static final String[] MOD_EXP;
    static final String[] DLY_EXP;
    static final String[] REV_EXP;
    static final String[] PIT_EXP;
    static final String[] COMP;
    static final String[] DRIVE;
    static final String[] DELAY;
    static final String[] REVERB;
    static final String[] MOD;
    static final String[] PITCH;
    static final String[] HILO;
    static final String[] LOHI;
    static final String[] HARDSOFT;
    static final String[] SOFTHARD;
    static final String[] UPDOWN;
    static final String[] DOWNUP;
    static final String[] ONOFF;
    static final String[] OFFON;
    static final String[] ROUTING;
    static final String[] EQWIDTH;
    static final String[] HICUT;
    static final String[] LOCUT;
    static final String[] RATIO;
    static final String[] RELEASE;
    static final String[] TEMPO;
    static final String[] SPEED;
    static final String[] EQFREQ;
    static final String[] ATTACK;
    static final String[] DEGREES;
    static final String[] KEY;
    static final String[] SCALE;
    static final String[] SHAPE;
    static final String[] SIZE;
    static final String[] HICOL;
    static final String[] LOCOL;
    static final String[] PEDAL;
    static final String[] PEDALMASTER;
    static final String[] TAPMASTER;
    static final String[] FOOTSWITCH;
    static final String[] INPUTSRC;
    static final String[] DCLOCK;
    static final String[] DITHER;
    static final String[] OUTPUTRANGE;
    static final String[] VOLUMEPOS;
    static final String[] TUNEROUT;
    static final String[] TUNERMODE;
    static final String[] TUNERRANGE;
    static final String[] IMPEDANCE;
    static String[] MIDI_CC;
    static String[] MIDI_CHANNEL;
    static String[] MIDI_SYSEX;
    static String[] PRESET;
    static String[] CPRESET;
    static String[] TYPE_N200_200;
    static String[] TYPE_N100_0;
    static String[] TYPE_N100_OFF_0;
    static String[] TYPE_N100_100;
    static String[] TYPE_N99_15;
    static String[] TYPE_N99_0;
    static String[] TYPE_N99_12;
    static String[] TYPE_N60_0;
    static String[] TYPE_N50_0;
    static String[] TYPE_N50_50;
    static String[] TYPE_N40_0;
    static String[] TYPE_N30_0;
    static String[] TYPE_N25_25;
    static String[] TYPE_N12_12;
    static String[] TYPE_0_10;
    static String[] TYPE_0_18;
    static String[] TYPE_0_24;
    static String[] TYPE_0_30;
    static String[] TYPE_0_50;
    static String[] TYPE_DECI_01_50;
    static String[] TYPE_0_90;
    static String[] TYPE_0_100;
    static String[] TYPE_0_120;
    static String[] TYPE_0_200;
    static String[] TYPE_0_1800;
    static final String[] TYPE_1_2;
    static String[] TYPE_1_10;
    static String[] TYPE_1_20;
    static String[] TYPE_3_200;
    static String[] TYPE_100_3000;
    static String[] TYPE_DECI_01_20;
    static String[] TYPE_0_350;
    static String[] TYPE_N2400_2400;
    static String[] TYPE_N100_6;
    static String[] TYPE_420_460;
    static String[] TYPE_N6_18;
    static final String[] ALLBANKS;
    static final String[] BANKS;
    static final String[] NUM;

    public static void main(String[] stringArray) {
        System.out.println("HICUT : " + HICUT.length);
        System.out.println("LOCUT : " + LOCUT.length);
        System.out.println("RATIO : " + RATIO.length);
        System.out.println("RELEASE : " + RELEASE.length);
        System.out.println("SPEED : " + SPEED.length);
        System.out.println("EQFREQ : " + EQFREQ.length);
    }

    static {
        int n;
        int n2;
        darkblue = new Color(13, 21, 31);
        grisvert = new Color(91, 106, 103);
        CC0 = new byte[]{-80, 2, 0};
        CC60 = new byte[]{-80, 2, 96};
        CC7F = new byte[]{-80, 2, 127};
        PC1 = new byte[]{-64, 1};
        PC2 = new byte[]{-64, 2};
        MAPPARAM = new String[]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19"};
        DRV_EXP = new String[]{"DRV Gain", "DRV Level"};
        MOD_EXP = new String[]{"CHO Speed", "CHO Depth", "CHO HiCut", "", "FLA Speed", "FLA Depth", "FLA FeedB", "", "VIB Speed", "VIB Depth", "VIB HiCut", "", "PHA Speed", "PHA Mix", "", "", "TREM Speed", "TREM Depth", "TREM HiCut", "TREM Width", "PAN Speed", "PAN Depth", "", ""};
        DLY_EXP = new String[]{"DLY Delay", "DLY FeedB", "DLY HiCut", "DLY Mix", "", "", "DLY Delay", "DLY FeedB", "DLY HiCut", "DLY Mix", "", "", "DLY Delay", "DLY FeedB", "DLY HiCut", "DLY Mix", "", "", "DLY Delay", "DLY FeedB", "DLY Mix", "", "", "", "DLY Dly1", "DLY Dly2", "DLY FB1", "DLY FB2", "DLY FBHCut", "DLY Mix", "DLY Delay", "DLY FeedB", "DLY HiCut", "DLY Mix", "", ""};
        REV_EXP = new String[]{"REV Decay", "REV PreDly", "REV Color", "REV Mix"};
        PIT_EXP = new String[]{"PIT Voic1", "PIT Voic2", "PIT Mix", "PIT Mix", "", "", "PIT Pitch", "", "", "PIT Voic1", "PIT Voic2", "PIT Mix", "PIT Voic1", "PIT Voic2", "PIT Mix"};
        COMP = new String[]{"perc", "sustain", "advance"};
        DRIVE = new String[]{"overdrive", "distorsion"};
        DELAY = new String[]{"clean", "analog", "tape", "dynam", "dual", "p.pong"};
        REVERB = new String[]{"spring", "hall", "room", "plate"};
        MOD = new String[]{"chorus", "flanger", "vibrato", "phaser", "tremolo", "panner"};
        PITCH = new String[]{"shifter", "octave", "wham", "detune", "intell P"};
        HILO = new String[]{"High", "Low"};
        LOHI = new String[]{"Low", "High"};
        HARDSOFT = new String[]{"Hard", "Soft"};
        SOFTHARD = new String[]{"Soft", "Hard"};
        UPDOWN = new String[]{"Up", "Down"};
        DOWNUP = new String[]{"Down", "Up"};
        ONOFF = new String[]{"On", "Off"};
        OFFON = new String[]{"Off", "On"};
        ROUTING = new String[]{"Serial", "SemiPar", "Parallel"};
        EQWIDTH = new String[]{"0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6"};
        HICUT = new String[]{"19.95", "22.39", "25.12", "28.18", "31.62", "35.48", "39.81", "44.67", "50.12", "56.23", "63.10", "70.79", "79.43", "89.13", "100.0", "112.2", "125.9", "141.3", "158.5", "177.8", "199.5", "223.9", "251.2", "281.8", "316.2", "354.8", "398.1", "446.7", "501.2", "562.3", "631.0", "707.9", "794.3", "891.3", "1.00k", "1.12k", "1.26k", "1.41k", "1.58k", "1.78k", "2.00k", "2.24k", "2.51k", "2.82k", "3.16k", "3.55k", "3.98k", "4.47k", "5.01k", "5.62k", "6.31k", "7.08k", "7.94k", "8.91k", "10.0k", "11.2k", "12.6k", "14.1k", "15.8k", "17.8k", "Off"};
        LOCUT = new String[]{"Off", "22.39", "25.12", "28.18", "31.62", "35.48", "39.81", "44.67", "50.12", "56.23", "63.10", "70.79", "79.43", "89.13", "100.0", "112.2", "125.9", "141.3", "158.5", "177.8", "199.5", "223.9", "251.2", "281.8", "316.2", "354.8", "398.1", "446.7", "501.2", "562.3", "631.0", "707.9", "794.3", "891.3", "1.00k", "1.12k", "1.26k", "1.41k", "1.58k", "1.78k", "2.00k"};
        RATIO = new String[]{"Off", "1.1:1", "1.3:1", "1.4:1", "1.6:1", "1.8:1", "2.0:1", "2.5:1", "3.2:1", "4.0:1", "5.6:1", "8.0:1", "16:1", "32:1", "64:1", "Inf:1"};
        RELEASE = new String[]{"1.0", "1.4", "2.0", "3.0", "5.0", "7.0", "10", "14", "20", "30", "50", "70", "100", "140", "200", "300", "500", "700", "1.0s", "1.4s", "2.0s"};
        TEMPO = new String[]{"Disabled", "1", "1/2D", "1/2", "1/2T", "1/4D", "1/4", "1/4T", "1/8D", "1/8", "1/8T", "1/16D", "1/16", "1/16T", "1/32D", "1/32", "1/32T"};
        SPEED = new String[]{".050", ".052", ".053", ".055", ".056", ".058", ".060", ".061", ".063", ".065", ".067", ".069", ".071", ".073", ".075", ".077", ".079", ".082", ".084", ".087", ".089", ".092", ".094", ".097", ".100", ".103", ".106", ".109", ".112", ".115", ".119", ".122", ".126", ".130", ".133", ".137", ".141", ".145", ".150", ".154", ".158", ".163", ".168", ".173", ".178", ".183", ".188", ".194", ".200", ".205", ".211", ".218", ".224", ".230", ".237", ".244", ".251", ".259", ".266", ".274", ".282", ".290", ".299", ".307", ".316", ".325", ".335", ".345", ".355", ".365", ".376", ".387", ".398", ".410", ".422", ".434", ".447", ".460", ".473", ".487", ".501", ".516", ".531", ".546", ".562", ".579", ".596", ".613", ".631", ".649", ".668", ".688", ".708", ".729", ".750", ".772", ".794", ".818", ".841", ".866", ".891", ".917", ".944", ".972", "1.00", "1.03", "1.06", "1.09", "1.12", "1.15", "1.19", "1.22", "1.26", "1.30", "1.33", "1.37", "1.41", "1.45", "1.50", "1.54", "1.58", "1.63", "1.68", "1.73", "1.78", "1.83", "1.88", "1.94", "2.00", "2.05", "2.11", "2.18", "2.24", "2.30", "2.37", "2.44", "2.51", "2.59", "2.66", "2.74", "2.82", "2.90", "2.99", "3.07", "3.16", "3.25", "3.35", "3.45", "3.55", "3.65", "3.76", "3.87", "3.98", "4.10", "4.22", "4.34", "4.47", "4.60", "4.73", "4.87", "5.01", "5.16", "5.31", "5.46", "5.62", "5.79", "5.96", "6.13", "6.31", "6.49", "6.68", "6.88", "7.08", "7.29", "7.50", "7.72", "7.94", "8.18", "8.41", "8.66", "8.91", "9.17", "9.44", "9.72", "10.00", "10.29", "10.59", "10.90", "11.22", "11.55", "11.89", "12.23", "12.59", "12.96", "13.34", "13.72", "14.13", "14.54", "14.96", "15.40", "15.85", "16.31", "16.79", "17.28", "17.78", "18.30", "18.84", "19.39", "19.95"};
        EQFREQ = new String[]{"41.0", "42.2", "43.4", "44.7", "46.0", "47.3", "48.7", "50.1", "51.6", "53.1", "54.6", "56.2", "57.9", "59.6", "61.3", "63.1", "64.9", "66.8", "68.8", "70.8", "72.9", "75.0", "77.2", "79.4", "81.8", "84.1", "86.6", "89.1", "91.7", "94.4", "97.2", "100", "103", "106", "109", "112", "115", "119", "122", "126", "130", "133", "137", "141", "145", "150", "154", "158", "163", "168", "173", "178", "183", "188", "194", "200", "205", "211", "218", "224", "230", "237", "244", "251", "259", "266", "274", "282", "290", "299", "307", "316", "325", "335", "345", "355", "365", "376", "387", "398", "410", "422", "434", "447", "460", "473", "487", "501", "516", "531", "546", "562", "579", "596", "613", "631", "649", "668", "688", "708", "729", "750", "772", "794", "818", "841", "866", "891", "917", "944", "972", "1.00k", "1.03k", "1.06k", "1.09k", "1.12k", "1.15k", "1.19k", "1.22k", "1.26k", "1.30k", "1.33k", "1.37k", "1.41k", "1.45k", "1.50k", "1.54k", "1.58k", "1.63k", "1.68k", "1.73k", "1.78k", "1.83k", "1.88k", "1.94k", "2.00k", "2.05k", "2.11k", "2.18k", "2.24k", "2.30k", "2.37k", "2.44k", "2.51k", "2.59k", "2.66k", "2.74k", "2.82k", "2.90k", "2.99k", "3.07k", "3.16k", "3.25k", "3.35k", "3.45k", "3.55k", "3.65k", "3.76k", "3.87k", "3.98k", "4.10k", "4.22k", "4.34k", "4.47k", "4.60k", "4.73k", "4.87k", "5.01k", "5.16k", "5.31k", "5.46k", "5.62k", "5.79k", "5.96k", "6.13k", "6.31k", "6.49k", "6.68k", "6.88k", "7.08k", "7.29k", "7.50k", "7.72k", "7.94k", "8.18k", "8.41k", "8.66k", "8.91k", "9.17k", "9.44k", "9.72k", "10.0k", "10.3k", "10.6k", "10.9k", "11.2k", "11.5k", "11.9k", "12.2k", "12.6k", "13.0k", "13.3k", "13.7k", "14.1k", "14.5k", "15.0k", "15.4k", "15.8k", "16.3k", "16.8k", "17.3k", "17.8k", "18.3k", "18.8k", "19.4k", "20.0k", "Off"};
        ATTACK = new String[]{"0.3", "0.5", "0.7", "1.0", "1.4", "2.0", "3", "5", "7", "10", "14", "20", "30", "50", "70", "100", "140"};
        DEGREES = new String[]{"-13", "-12", "-11", "-10", "-9", "-oct", "-7", "-6", "-5", "-4", "-3", "-2", "Uniss", "+2", "+3", "+4", "+5", "+6", "+7", "+oct", "+9", "+10", "+11", "+12", "+13"};
        KEY = new String[]{"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
        SCALE = new String[]{"Ionian", "Dorian", "Phrygian", "Lydian", "Mixolyd", "Aeolian", "Locrian", "PntMin", "PntMaj", "Blues", "DimWhl", "Whole", "HrmMin"};
        SHAPE = new String[]{"Round", "Curved", "Square"};
        SIZE = new String[]{"Box", "Tiny", "Small", "Medium", "Large", "XL", "Grand", "Huge"};
        HICOL = new String[]{"Wool", "Warm", "Real", "Clear", "Bright", "Crisp", "Glass"};
        LOCOL = new String[]{"Thick", "Round", "Real", "Light", "Tight", "Thin", "NoBass"};
        PEDAL = new String[]{"Expression", "G-Switch", "Exp.GlbVol"};
        PEDALMASTER = new String[]{"Preset", "Pedal"};
        TAPMASTER = new String[]{"Preset", "Tap"};
        FOOTSWITCH = new String[]{"Pedal", "Preset"};
        INPUTSRC = new String[]{"Line", "Drive", "Digital"};
        DCLOCK = new String[]{"44.1kHz", "48kHz", "Digital"};
        DITHER = new String[]{"Off", "20", "16", "8"};
        OUTPUTRANGE = new String[]{"2", "8", "14", "20"};
        VOLUMEPOS = new String[]{"Pre", "Post"};
        TUNEROUT = new String[]{"Mute", "On"};
        TUNERMODE = new String[]{"Coarse", "Fine"};
        TUNERRANGE = new String[]{"Guitar", "Bass", "7strGuit"};
        IMPEDANCE = new String[]{"Lo-Z", "Hi-Z"};
        MIDI_CC = new String[129];
        MIDI_CHANNEL = new String[18];
        MIDI_SYSEX = new String[128];
        PRESET = new String[91];
        CPRESET = new String[90];
        TYPE_N200_200 = new String[401];
        TYPE_N100_0 = new String[101];
        TYPE_N100_OFF_0 = new String[101];
        TYPE_N100_100 = new String[201];
        TYPE_N99_15 = new String[115];
        TYPE_N99_0 = new String[100];
        TYPE_N99_12 = new String[112];
        TYPE_N60_0 = new String[100];
        TYPE_N50_0 = new String[51];
        TYPE_N50_50 = new String[101];
        TYPE_N40_0 = new String[41];
        TYPE_N30_0 = new String[31];
        TYPE_N25_25 = new String[51];
        TYPE_N12_12 = new String[25];
        TYPE_0_10 = new String[11];
        TYPE_0_18 = new String[19];
        TYPE_0_24 = new String[25];
        TYPE_0_30 = new String[31];
        TYPE_0_50 = new String[51];
        TYPE_DECI_01_50 = new String[500];
        TYPE_0_90 = new String[91];
        TYPE_0_100 = new String[101];
        TYPE_0_120 = new String[121];
        TYPE_0_200 = new String[201];
        TYPE_0_1800 = new String[1801];
        TYPE_1_2 = new String[]{"1", "2"};
        TYPE_1_10 = new String[10];
        TYPE_1_20 = new String[20];
        TYPE_3_200 = new String[198];
        TYPE_100_3000 = new String[2901];
        TYPE_DECI_01_20 = new String[200];
        TYPE_0_350 = new String[351];
        TYPE_N2400_2400 = new String[4801];
        TYPE_N100_6 = new String[107];
        TYPE_420_460 = new String[41];
        TYPE_N6_18 = new String[25];
        ALLBANKS = new String[]{"F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19"};
        BANKS = new String[]{"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19"};
        NUM = new String[]{"1", "2", "3"};
        for (n2 = 0; n2 <= 127; ++n2) {
            Constants.MIDI_CC[n2 + 1] = "" + n2;
        }
        Constants.MIDI_CC[0] = "Off";
        for (n2 = 1; n2 < 18; ++n2) {
            Constants.MIDI_CHANNEL[n2] = "" + n2;
        }
        Constants.MIDI_CHANNEL[0] = "Off";
        Constants.MIDI_CHANNEL[17] = "Omni";
        for (n2 = 0; n2 < 127; ++n2) {
            Constants.MIDI_SYSEX[n2] = "" + n2;
        }
        Constants.MIDI_SYSEX[127] = "All";
        Constants.PRESET[0] = "current";
        for (n2 = 0; n2 < 10; ++n2) {
            for (n = 1; n < 4; ++n) {
                Constants.PRESET[3 * n2 + n] = "F" + n2 + "-" + n;
                Constants.CPRESET[3 * n2 + n - 1] = "F" + n2 + "-" + n;
            }
        }
        for (n2 = 10; n2 < 30; ++n2) {
            for (n = 1; n < 4; ++n) {
                int n3 = n2 - 10;
                String string = "";
                if (n3 < 10) {
                    string = "0";
                }
                Constants.PRESET[3 * n2 + n] = string + n3 + "-" + n;
                Constants.CPRESET[3 * n2 + n - 1] = string + n3 + "-" + n;
            }
        }
        for (n2 = -2400; n2 <= 2400; ++n2) {
            Constants.TYPE_N2400_2400[n2 + 2400] = "" + n2;
        }
        for (n2 = -200; n2 <= 200; ++n2) {
            Constants.TYPE_N200_200[n2 + 200] = "" + n2;
        }
        for (n2 = -100; n2 <= 0; ++n2) {
            Constants.TYPE_N100_0[n2 + 100] = "" + n2;
        }
        for (n2 = -100; n2 <= 0; ++n2) {
            Constants.TYPE_N100_OFF_0[n2 + 100] = "" + n2;
        }
        Constants.TYPE_N100_OFF_0[0] = "Off";
        for (n2 = -100; n2 <= 100; ++n2) {
            Constants.TYPE_N100_100[n2 + 100] = "" + n2;
        }
        for (n2 = -99; n2 <= 15; ++n2) {
            Constants.TYPE_N99_15[n2 + 99] = "" + n2;
        }
        for (n2 = -99; n2 <= 0; ++n2) {
            Constants.TYPE_N99_0[n2 + 99] = "" + n2;
        }
        for (n2 = -99; n2 <= 12; ++n2) {
            Constants.TYPE_N99_12[n2 + 99] = "" + n2;
        }
        for (n2 = -60; n2 <= 0; ++n2) {
            Constants.TYPE_N60_0[n2 + 60] = "" + n2;
        }
        for (n2 = -50; n2 <= 50; ++n2) {
            Constants.TYPE_N50_50[n2 + 50] = "" + n2;
        }
        for (n2 = -50; n2 <= 0; ++n2) {
            Constants.TYPE_N50_0[n2 + 50] = "" + n2;
        }
        for (n2 = -40; n2 <= 0; ++n2) {
            Constants.TYPE_N40_0[n2 + 40] = "" + n2;
        }
        for (n2 = -30; n2 <= 0; ++n2) {
            Constants.TYPE_N30_0[n2 + 30] = "" + n2;
        }
        for (n2 = -25; n2 <= 25; ++n2) {
            Constants.TYPE_N25_25[n2 + 25] = "" + n2;
        }
        for (n2 = -12; n2 <= 12; ++n2) {
            Constants.TYPE_N12_12[n2 + 12] = "" + n2;
        }
        for (n2 = 0; n2 <= 10; ++n2) {
            Constants.TYPE_0_10[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 18; ++n2) {
            Constants.TYPE_0_18[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 24; ++n2) {
            Constants.TYPE_0_24[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 30; ++n2) {
            Constants.TYPE_0_30[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 50; ++n2) {
            Constants.TYPE_0_50[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 90; ++n2) {
            Constants.TYPE_0_90[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 100; ++n2) {
            Constants.TYPE_0_100[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 120; ++n2) {
            Constants.TYPE_0_120[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 200; ++n2) {
            Constants.TYPE_0_200[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 350; ++n2) {
            Constants.TYPE_0_350[n2] = "" + n2;
        }
        for (n2 = 0; n2 <= 1800; ++n2) {
            Constants.TYPE_0_1800[n2] = "" + n2;
        }
        for (n2 = 1; n2 <= 10; ++n2) {
            Constants.TYPE_1_10[n2 - 1] = "" + n2;
        }
        for (n2 = 1; n2 <= 20; ++n2) {
            Constants.TYPE_1_20[n2 - 1] = "" + n2;
        }
        for (n2 = 3; n2 <= 200; ++n2) {
            Constants.TYPE_3_200[n2 - 3] = "" + n2;
        }
        for (n2 = 100; n2 <= 3000; ++n2) {
            Constants.TYPE_100_3000[n2 - 100] = "" + n2;
        }
        for (n2 = -100; n2 <= 6; ++n2) {
            Constants.TYPE_N100_6[n2 + 100] = "" + n2;
        }
        for (n2 = 420; n2 <= 460; ++n2) {
            Constants.TYPE_420_460[n2 - 420] = "" + n2;
        }
        for (n2 = 1; n2 <= 500; ++n2) {
            Constants.TYPE_DECI_01_50[n2 - 1] = "" + n2 / 10 + "." + n2 % 10;
        }
        for (n2 = 1; n2 <= 200; ++n2) {
            Constants.TYPE_DECI_01_20[n2 - 1] = "" + n2 / 10 + "." + n2 % 10;
        }
        for (n2 = -6; n2 <= 18; ++n2) {
            Constants.TYPE_N6_18[n2 + 6] = "" + n2;
        }
    }
}
