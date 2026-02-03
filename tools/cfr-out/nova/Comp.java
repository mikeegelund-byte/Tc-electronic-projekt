/*
 * Decompiled with CFR 0.152.
 */
package nova;

import javax.swing.Box;
import nova.CopyBlock;
import nova.EmptyRow;
import nova.Nibble;
import nova.Patch;
import nova.Row;

public class Comp
extends CopyBlock {
    Nibble type;
    Nibble threshold;
    Nibble ratio;
    Nibble attack;
    Nibble release;
    Nibble response;
    Nibble drive;
    Nibble level;
    Nibble onoff;

    public Comp(Patch patch) {
        super("Compressor", patch, true);
        this.activeNibbles = new int[]{24, 9, 10, 11, 12, 13, 14, 15, 16};
        try {
            for (int i = 0; i < this.activeNibbles.length; ++i) {
                this.nibbles.add(this.parent.getNibble(this.activeNibbles[i]));
            }
            this.onoff = this.parent.getNibble(24);
            this.type = this.parent.getNibble(9);
            this.threshold = this.parent.getNibble(10);
            this.ratio = this.parent.getNibble(11);
            this.attack = this.parent.getNibble(12);
            this.release = this.parent.getNibble(13);
            this.response = this.parent.getNibble(14);
            this.drive = this.parent.getNibble(15);
            this.level = this.parent.getNibble(16);
            this.type.updateNibble("Type", 50);
            this.onoff.updateNibble("On", 2);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Comp Block");
            exception.printStackTrace();
        }
        this.refresh();
    }

    void refresh() {
        int n;
        this.box.removeAll();
        for (n = 2; n < this.activeNibbles.length; ++n) {
            Nibble nibble = this.parent.getNibble(this.activeNibbles[n]);
            nibble.setType(100);
        }
        n = 0;
        this.box.add(new Row("Compressor", this.onoff, n++));
        this.box.add(new Row("Comp", this.type, n++));
        if (this.type.getValue() < 2) {
            this.activeNibbles = new int[]{24, 9, 15, 14, 16};
            this.drive.updateNibble("Drive", 122);
            this.response.updateNibble("Response", 121);
            this.level.updateNibble("Level (dB)", 133);
            this.box.add(new Row("Comp", this.drive, n++));
            this.box.add(new Row("Comp", this.response, n++));
            this.box.add(new Row("Comp", this.level, n++));
        } else {
            this.activeNibbles = new int[]{24, 9, 10, 11, 12, 13, 16};
            this.threshold.updateNibble("Threshold (dB)", 114);
            this.ratio.updateNibble("Ratio", 11);
            this.release.updateNibble("Release (ms)", 12);
            this.attack.updateNibble("Attack (ms)", 10);
            this.level.updateNibble("Level (dB)", 133);
            this.box.add(new Row("Comp", this.threshold, n++));
            this.box.add(new Row("Comp", this.ratio, n++));
            this.box.add(new Row("Comp", this.attack, n++));
            this.box.add(new Row("Comp", this.release, n++));
            this.box.add(new Row("Comp", this.level, n++));
        }
        while (n < 9) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 2;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
