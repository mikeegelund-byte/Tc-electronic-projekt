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
import nova.EmptyRow;
import nova.MidiMapInRow;
import nova.Nibble;
import nova.Row;
import nova.SystemDump;

public class MidiMapIn
extends Block {
    String name = "";
    JPanel p;

    public MidiMapIn(SystemDump systemDump) {
        super("MIDI Map In", systemDump);
        try {
            for (int i = 0; i <= 42; ++i) {
                Nibble nibble = this.system.getNibble(64 + i);
                if (i < 42) {
                    nibble.updateNibble("" + (3 * i + 1) + "-" + (3 * i + 3), 73);
                } else {
                    nibble.updateNibble("" + (3 * i + 1), 73);
                }
                this.nibbles.add(nibble);
            }
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load MIDI MAP IN Block");
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
        this.add((Component)new Row("MIDI MAP IN", null, 0), gridBagConstraints);
        ++gridBagConstraints.gridx;
        for (n = 0; n < 4; ++n) {
            if (n == 4) {
                gridBagConstraints.weightx = 1.0;
            }
            this.add((Component)new Row("", null, 0), gridBagConstraints);
            ++gridBagConstraints.gridx;
        }
        for (n = 0; n <= this.nibbles.size() / 5; ++n) {
            gridBagConstraints.gridx = 0;
            ++gridBagConstraints.gridy;
            gridBagConstraints.weightx = 0.0;
            for (int i = 0; i < 5; ++i) {
                if (i == 4) {
                    gridBagConstraints.weightx = 1.0;
                }
                if (5 * n + i >= this.nibbles.size()) {
                    this.add((Component)new EmptyRow(false, 1), gridBagConstraints);
                } else {
                    this.add((Component)new MidiMapInRow(5 * n + i, (Nibble)this.nibbles.elementAt(5 * n + i), n + 1), gridBagConstraints);
                }
                ++gridBagConstraints.gridx;
            }
        }
    }

    public String toHTML() {
        String string = "";
        string = string + "<H2>MIDI Map In</H2>\n";
        string = string + "<TABLE class=MIDI border=0 cols=24 cellspacing=5 width=\"1000\">\n";
        for (int i = 0; i <= this.nibbles.size() / 5; ++i) {
            string = string + "<TR>\n";
            for (int j = 0; j < 5; ++j) {
                int n = 5 * i + j;
                if (n >= this.nibbles.size()) continue;
                Nibble nibble = (Nibble)this.nibbles.elementAt(n);
                int n2 = nibble.getMapMidiValue1();
                String string2 = n2 > 0 && n2 < 91 ? Constants.PRESET[n2] : "None";
                string = string + "<TD align=center>\n";
                string = string + "&nbsp;" + (3 * n + 1) + "&nbsp;>&nbsp;" + string2 + "&nbsp;";
                string = string + "</TD>\n";
                string = string + "<TD align=center>\n";
                if (3 * n + 2 < 128) {
                    n2 = nibble.getMapMidiValue2();
                    string2 = n2 > 0 && n2 < 91 ? Constants.PRESET[n2] : "None";
                    string = string + "&nbsp;" + (3 * n + 2) + "&nbsp;>&nbsp;" + string2 + "&nbsp;";
                }
                string = string + "</TD>\n";
                string = string + "<TD align=center>\n";
                if (3 * n + 3 < 128) {
                    n2 = nibble.getMapMidiValue3();
                    string2 = n2 > 0 && n2 < 91 ? Constants.PRESET[n2] : "None";
                    string = string + "&nbsp;" + (3 * n + 3) + "&nbsp;>&nbsp;" + string2 + "&nbsp;";
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
        string = string + "<block name=\"MIDI Map In\">\n";
        for (int i = 0; i < this.nibbles.size(); ++i) {
            Nibble nibble = (Nibble)this.nibbles.elementAt(i);
            string = string + nibble.toXMLOpen();
            int n = nibble.getMapMidiValue1();
            String string2 = n > 0 && n < 91 ? Constants.PRESET[n] : "None";
            string = string + string2;
            if (3 * i + 2 < 128) {
                n = nibble.getMapMidiValue2();
                string2 = n > 0 && n < 91 ? Constants.PRESET[n] : "None";
                string = string + ", " + string2;
            }
            if (3 * i + 3 < 128) {
                n = nibble.getMapMidiValue3();
                string2 = n > 0 && n < 91 ? Constants.PRESET[n] : "None";
                string = string + ", " + string2;
            }
            string = string + nibble.toXMLClose();
        }
        string = string + "</block>\n";
        return string;
    }
}
