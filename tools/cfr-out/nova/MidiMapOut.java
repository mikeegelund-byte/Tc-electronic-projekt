/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.Insets;
import javax.swing.JPanel;
import nova.Block;
import nova.Constants;
import nova.MidiMapOutRow;
import nova.Nibble;
import nova.Row;
import nova.SystemDump;

public class MidiMapOut
extends Block {
    String name = "";
    JPanel p;

    public MidiMapOut(SystemDump systemDump) {
        super("MIDI Map Out", systemDump);
        try {
            for (int i = 0; i < 20; ++i) {
                Nibble nibble = this.system.getNibble(107 + i);
                nibble.updateNibble("" + i, 73);
                this.nibbles.add(nibble);
            }
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load MIDI MAP OUT Block");
            System.err.println(this.system.getNibbleCount());
        }
        this.setBorder(border);
        this.refresh();
    }

    void refresh() {
        int n;
        this.removeAll();
        GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.insets = new Insets(0, 0, 0, 0);
        gridBagConstraints.weightx = 0.0;
        gridBagConstraints.fill = 2;
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        this.add((Component)new Row("MIDI MAP OUT", null, 0), gridBagConstraints);
        ++gridBagConstraints.gridx;
        for (n = 0; n < 4; ++n) {
            if (n == 4) {
                gridBagConstraints.weightx = 1.0;
            }
            this.add((Component)new Row("", null, 0), gridBagConstraints);
            ++gridBagConstraints.gridx;
        }
        for (n = 0; n < this.nibbles.size() / 5; ++n) {
            gridBagConstraints.gridx = 0;
            ++gridBagConstraints.gridy;
            gridBagConstraints.weightx = 0.0;
            for (int i = 0; i < 5; ++i) {
                if (i == 4) {
                    gridBagConstraints.weightx = 1.0;
                }
                this.add((Component)new MidiMapOutRow(5 * n + i, (Nibble)this.nibbles.elementAt(5 * n + i), n + 1), gridBagConstraints);
                ++gridBagConstraints.gridx;
            }
        }
    }

    public String toHTML() {
        String string = "";
        string = string + "<H2>MIDI Map Out</H2>\n";
        string = string + "<TABLE class=MIDI border=0 cols=24 cellspacing=5 width=\"1000\">\n";
        for (int i = 0; i <= this.nibbles.size() / 4; ++i) {
            string = string + "<TR>\n";
            for (int j = 0; j < 4; ++j) {
                int n = 4 * i + j;
                if (n >= this.nibbles.size()) continue;
                Nibble nibble = (Nibble)this.nibbles.elementAt(n);
                int n2 = nibble.getMapMidiValue1();
                String string2 = Constants.PRESET[31 + 3 * n];
                string = string + "<TD align=center>\n";
                string = string + "&nbsp;" + string2 + "&nbsp;>&nbsp;" + n2 + "&nbsp;";
                string = string + "</TD>\n";
                string = string + "<TD align=center>\n";
                if (3 * n + 2 < 128) {
                    n2 = nibble.getMapMidiValue2();
                    string2 = Constants.PRESET[32 + 3 * n];
                    string = string + "&nbsp;" + string2 + "&nbsp;>&nbsp;" + n2 + "&nbsp;";
                }
                string = string + "</TD>\n";
                string = string + "<TD align=center>\n";
                if (3 * n + 3 < 128) {
                    n2 = nibble.getMapMidiValue3();
                    string2 = Constants.PRESET[33 + 3 * n];
                    string = string + "&nbsp;" + string2 + "&nbsp;>&nbsp;" + n2 + "&nbsp;";
                }
                string = string + "</TD>\n";
            }
            string = string + "</TR>\n";
        }
        string = string + "</TABLE>\n";
        return string;
    }

    public String toXML() {
        String string = "";
        string = string + "<block name=\"MIDI Map Out\">\n";
        for (int i = 0; i < this.nibbles.size(); ++i) {
            Nibble nibble = (Nibble)this.nibbles.elementAt(i);
            string = string + nibble.toXMLOpen();
            int n = nibble.getMapMidiValue1();
            string = string + n;
            if (3 * i + 2 < 128) {
                n = nibble.getMapMidiValue2();
                string = string + ", " + n;
            }
            if (3 * i + 3 < 128) {
                n = nibble.getMapMidiValue3();
                string = string + ", " + n;
            }
            string = string + nibble.toXMLClose();
        }
        string = string + "</block>\n";
        return string;
    }
}
