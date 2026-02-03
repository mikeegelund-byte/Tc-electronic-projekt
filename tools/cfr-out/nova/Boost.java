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

public class Boost
extends CopyBlock {
    Nibble level;
    Nibble onoff;

    public Boost(Patch patch) {
        super("Boost", patch, true);
        try {
            this.level = this.parent.getNibble(37);
            this.level.updateNibble("Level (dB)", 111);
            this.nibbles.add(this.level);
            this.onoff = this.parent.getNibble(38);
            this.onoff.updateNibble("On", 2);
            this.nibbles.add(this.onoff);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Boost Block");
        }
        this.refresh();
    }

    void refresh() {
        this.box.removeAll();
        int n = 0;
        this.activeNibbles = new int[]{38, 37};
        this.box.add(new Row("Boost", this.onoff, n++));
        this.box.add(new Row("Boost", this.level, n++));
        while (n < 9) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 9;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
