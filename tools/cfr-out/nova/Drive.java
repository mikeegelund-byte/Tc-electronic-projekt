/*
 * Decompiled with CFR 0.152.
 */
package nova;

import javax.swing.Box;
import javax.swing.JLabel;
import javax.swing.JTextField;
import nova.CopyBlock;
import nova.EmptyRow;
import nova.Nibble;
import nova.Patch;
import nova.Row;

public class Drive
extends CopyBlock {
    String name = "";
    int bankN;
    int presN;
    JLabel bankLbl;
    JLabel presLbl;
    JTextField bankTf;
    JTextField presTf;
    JTextField nameTf;
    Nibble type;
    Nibble gain;
    Nibble tone;
    Nibble level;
    Nibble onoff;

    public Drive(Patch patch) {
        super("Drive", patch, true);
        try {
            this.type = this.parent.getNibble(25);
            this.type.updateNibble("Type", 51);
            this.nibbles.add(this.type);
            this.gain = this.parent.getNibble(26);
            this.gain.updateNibble("Gain (dB)", 113);
            this.nibbles.add(this.gain);
            this.tone = this.parent.getNibble(27);
            this.tone.updateNibble("Tone (%)", 116);
            this.nibbles.add(this.tone);
            this.level = this.parent.getNibble(39);
            this.level.updateNibble("Level (dB)", 104);
            this.nibbles.add(this.level);
            this.onoff = this.parent.getNibble(40);
            this.onoff.updateNibble("On", 2);
            this.nibbles.add(this.onoff);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Drive Block");
        }
        this.refresh();
    }

    void refresh() {
        this.box.removeAll();
        int n = 0;
        this.activeNibbles = new int[]{40, 25, 26, 27, 39};
        this.box.add(new Row("Drive", this.onoff, n++));
        this.box.add(new Row("", this.type, n++));
        this.box.add(new Row("", this.gain, n++));
        this.box.add(new Row("", this.tone, n++));
        this.box.add(new Row("", this.level, n++));
        while (n < 9) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 1;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
