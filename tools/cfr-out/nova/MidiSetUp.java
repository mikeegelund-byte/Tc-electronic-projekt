/*
 * Decompiled with CFR 0.152.
 */
package nova;

import nova.Block;
import nova.SystemDump;

public class MidiSetUp
extends Block {
    public MidiSetUp(SystemDump systemDump) {
        super("MIDI SetUp", systemDump);
        this.rowspan = 1;
        this.activeNibbles = new int[]{19, 20, 21, 23, 24, 22};
        for (int i = 0; i < this.activeNibbles.length; ++i) {
            this.nibbles.add(this.system.getNibble(this.activeNibbles[i]));
        }
    }

    void refresh() {
    }
}
