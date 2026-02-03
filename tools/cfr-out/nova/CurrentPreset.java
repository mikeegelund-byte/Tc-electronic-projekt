/*
 * Decompiled with CFR 0.152.
 */
package nova;

import nova.Block;
import nova.SystemDump;

public class CurrentPreset
extends Block {
    public CurrentPreset(SystemDump systemDump) {
        super("Current Preset", systemDump);
        this.activeNibbles = new int[]{57};
        for (int i = 0; i < this.activeNibbles.length; ++i) {
            this.nibbles.add(this.system.getNibble(this.activeNibbles[i]));
        }
    }

    void refresh() {
    }
}
