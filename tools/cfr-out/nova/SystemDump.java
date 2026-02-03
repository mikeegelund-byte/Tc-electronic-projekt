/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Calendar;
import java.util.List;
import java.util.Vector;
import javax.swing.Box;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JComponent;
import javax.swing.JFileChooser;
import javax.swing.JPanel;
import javax.swing.JRootPane;
import nova.Block;
import nova.Constants;
import nova.CurrentPreset;
import nova.EmptyRow;
import nova.Global;
import nova.Levels;
import nova.MidiCC;
import nova.MidiInterface;
import nova.MidiMapIn;
import nova.MidiMapOut;
import nova.MidiSetUp;
import nova.Nibble;
import nova.NibbleException;
import nova.NovaManager;
import nova.Pedal;
import nova.PrintablePanel;
import nova.Routing;
import nova.Row;
import nova.TapTempo;
import nova.Tuner;
import nova.Utility;

public class SystemDump
extends PrintablePanel
implements ActionListener {
    private static final int DUMP_SIZE = 526;
    private static final int DATA_START = 8;
    private static final int DATA_END = 523;
    private static final int CHECKSUM_POS = 524;
    private byte[] data;
    private String preset_name;
    private int message_id;
    private int checksum;
    private int bankN;
    private int presN;
    private int numcode;
    private File saveFile;
    private MidiMapIn mapin;
    private MidiMapOut mapout;
    static final String[] NIB_NAMES = new String[]{"Signature", "RoutingType", "ByPass", "Vol.Min (%)", "Vol.Mid (%)", "Vol.Max (%)", "PedalType", "Master", "CC TapTempo", "CC Comp", "CC Drv", "CC Mod", "CC Delay", "CC Rev", "CC NG", "CC Pitch", "CC EQ", "CC Boost", "CC Exp.Pedal", "MIDI Channel", "PrgChange.In", "PrgChange.Out", "Midi Clock", "SysEx ID", "Midi Sync", "#25", "#26", "TapLED Defeat", "Tap Master", "Boost Lock", "EQ Lock", "Routing Lock", "Factory Lock", "SpeakerSim", "AngleView", "FootSwitch", "Input Source", "Digital Clock", "#38", "FX Mute", "Dither", "Tempo", "Digital InGain (dB)", "Input InGain (dB)", "Advanced Mode", "Line Input Range (dB)", "Instr. Input Range (dB)", "BoostMax (dB)", "Output Range (dBu)", "Volume Level (dB)", "Volume Position", "KillDry", "Tuner Out", "Tuner Ref (Hz)", "Tuner Mode", "Tuner Range", "Send Tuner", "Current Preset", "#58", "edited", "#60", "Impedance", "Calibration Min", "Calibration Max"};
    static final int[] NIB_TYPES = new int[]{100, 3, 5, 116, 116, 116, 60, 61, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 71, 5, 5, 5, 72, 5, 100, 100, 5, 62, 5, 5, 5, 5, 5, 116, 63, 64, 65, 100, 27, 66, 123, 129, 102, 5, 112, 131, 111, 67, 102, 68, 5, 69, 130, 74, 75, 100, 7, 100, 100, 100, 76, 100, 100};
    private final int NIBBLE_LENGTH = 129;
    private Vector<Nibble> nibbles;
    JRootPane pane;
    Global global;
    JButton closeButton = new JButton("Close");
    JButton saveButton = new JButton("Save SysEx");
    JButton saveXML = new JButton("Export XML");
    JButton saveHTML = new JButton("Export HTML");
    JComboBox send = new JComboBox();
    String title;
    Vector<Block> blocks = new Vector();
    NovaManager parent;

    public SystemDump(NovaManager novaManager, byte[] byArray) {
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        Calendar calendar = Calendar.getInstance();
        this.title = "" + calendar.get(11);
        this.title = this.title + ":" + calendar.get(12);
        this.title = this.title + ":" + calendar.get(13);
        this.data = byArray;
        this.init();
    }

    public SystemDump(NovaManager novaManager, String string, byte[] byArray) {
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        long l = System.currentTimeMillis();
        this.title = string;
        this.data = byArray;
        this.init();
    }

    private void init() {
        JComponent jComponent;
        Object object;
        int n;
        this.setName(this.title);
        this.nibbles = new Vector();
        try {
            for (n = 0; n < 129; ++n) {
                object = new byte[4];
                for (int i = 0; i < 4; ++i) {
                    object[i] = this.data[8 + 4 * n + i];
                }
                jComponent = new Nibble(this, (byte[])object, 8 + 4 * n);
                if (n < 64) {
                    ((Nibble)jComponent).updateNibble(NIB_NAMES[n], NIB_TYPES[n]);
                }
                this.nibbles.add((Nibble)jComponent);
            }
        }
        catch (NibbleException nibbleException) {
            this.parent.log(nibbleException.getMessage());
        }
        this.blocks.add(new Levels(this));
        this.blocks.add(new Pedal(this));
        this.blocks.add(new Routing(this));
        this.blocks.add(new CurrentPreset(this));
        this.blocks.add(new Utility(this));
        this.blocks.add(new TapTempo(this));
        this.blocks.add(new MidiCC(this));
        this.blocks.add(new MidiSetUp(this));
        this.blocks.add(new Tuner(this));
        n = 14;
        jComponent = Box.createVerticalBox();
        jComponent.setBorder(Block.border);
        int n2 = 0;
        jComponent.add(new Row("levels", null, n2++));
        jComponent.add(new Row("volume", this.nibbles.elementAt(49), n2++));
        jComponent.add(new Row("volpos", this.nibbles.elementAt(50), n2++));
        jComponent.add(new Row("inputsrc", this.nibbles.elementAt(36), n2++));
        jComponent.add(new Row("inputingain", this.nibbles.elementAt(43), n2++));
        jComponent.add(new Row("LineInputRange", this.nibbles.elementAt(45), n2++));
        jComponent.add(new Row("InstrInputRange", this.nibbles.elementAt(46), n2++));
        jComponent.add(new Row("advmode", this.nibbles.elementAt(44), n2++));
        jComponent.add(new Row("boostmax", this.nibbles.elementAt(47), n2++));
        jComponent.add(new Row("output range", this.nibbles.elementAt(48), n2++));
        jComponent.add(new Row("digital clock", this.nibbles.elementAt(37), n2++));
        jComponent.add(new Row("digital ingain", this.nibbles.elementAt(42), n2++));
        jComponent.add(new Row("dither", this.nibbles.elementAt(40), n2++));
        while (n2 < n) {
            jComponent.add(new EmptyRow(false, n2++));
        }
        Box box = Box.createVerticalBox();
        box.setBorder(Block.border);
        n2 = 0;
        box.add(new Row("Pedal", null, n2++));
        box.add(new Row("master", this.nibbles.elementAt(7), n2++));
        box.add(new Row("pedaltype", this.nibbles.elementAt(6), n2++));
        box.add(new Row("volmin", this.nibbles.elementAt(3), n2++));
        box.add(new Row("volmid", this.nibbles.elementAt(4), n2++));
        box.add(new Row("volmax", this.nibbles.elementAt(5), n2++));
        object = this.nibbles.elementAt(61);
        ((Nibble)object).setEditable(false);
        box.add(new Row("imp", (Nibble)object, n2++));
        object = this.nibbles.elementAt(62);
        ((Nibble)object).setEditable(false);
        box.add(new Row("calibMin", (Nibble)object, n2++));
        object = this.nibbles.elementAt(63);
        ((Nibble)object).setEditable(false);
        box.add(new Row("calibMax", (Nibble)object, n2++));
        box.add(new Row("Routing", null, 0));
        int n3 = ++n2;
        box.add(new Row("routing type", this.nibbles.elementAt(1), n3));
        int n4 = ++n2;
        box.add(new Row("bypass", this.nibbles.elementAt(2), n4));
        int n5 = ++n2;
        ++n2;
        box.add(new Row("killdry", this.nibbles.elementAt(51), n5));
        while (n2 < n) {
            box.add(new EmptyRow(false, n2++));
        }
        Box box2 = Box.createVerticalBox();
        box2.setBorder(Block.border);
        n2 = 0;
        box2.add(new Row("Utility", null, n2++));
        box2.add(new Row("fx mute", this.nibbles.elementAt(39), n2++));
        box2.add(new Row("boost lock", this.nibbles.elementAt(29), n2++));
        box2.add(new Row("eq lock", this.nibbles.elementAt(30), n2++));
        box2.add(new Row("routing lock", this.nibbles.elementAt(31), n2++));
        box2.add(new Row("factory lock", this.nibbles.elementAt(32), n2++));
        box2.add(new Row("SpeakerSim", this.nibbles.elementAt(33), n2++));
        box2.add(new Row("footswitch", this.nibbles.elementAt(35), n2++));
        box2.add(new Row("angle view", this.nibbles.elementAt(34), n2++));
        box2.add(new Row("Tap Tempo", null, 0));
        int n6 = ++n2;
        box2.add(new Row("tap master", this.nibbles.elementAt(28), n6));
        int n7 = ++n2;
        box2.add(new Row("tap tempo", this.nibbles.elementAt(41), n7));
        int n8 = ++n2;
        ++n2;
        box2.add(new Row("TapLED Def", this.nibbles.elementAt(27), n8));
        while (n2 < n) {
            box2.add(new EmptyRow(false, n2++));
        }
        Box box3 = Box.createVerticalBox();
        box3.setBorder(Block.border);
        n2 = 0;
        box3.add(new Row("MIDI SetUp", null, n2++));
        box3.add(new Row("MIDI CHN", this.nibbles.elementAt(19), n2++));
        box3.add(new Row("MIDI PCI", this.nibbles.elementAt(20), n2++));
        box3.add(new Row("MIDI PCO", this.nibbles.elementAt(21), n2++));
        box3.add(new Row("MIDI SYS", this.nibbles.elementAt(23), n2++));
        box3.add(new Row("MIDI SYN", this.nibbles.elementAt(24), n2++));
        box3.add(new Row("MIDI CLOCK", this.nibbles.elementAt(22), n2++));
        box3.add(new EmptyRow(false, n2++));
        box3.add(new EmptyRow(false, n2++));
        box3.add(new Row("Tuner", null, 0));
        int n9 = ++n2;
        box3.add(new Row("tuner out", this.nibbles.elementAt(52), n9));
        int n10 = ++n2;
        box3.add(new Row("tuner ref", this.nibbles.elementAt(53), n10));
        int n11 = ++n2;
        box3.add(new Row("tuner mode", this.nibbles.elementAt(54), n11));
        int n12 = ++n2;
        ++n2;
        box3.add(new Row("tuner range", this.nibbles.elementAt(55), n12));
        while (n2 < n) {
            box3.add(new EmptyRow(false, n2++));
        }
        Box box4 = Box.createVerticalBox();
        box4.setBorder(Block.border);
        n2 = 0;
        box4.add(new Row("MIDI CC", null, n2++));
        box4.add(new Row("MIDI TT", this.nibbles.elementAt(8), n2++));
        box4.add(new Row("MIDI DRV", this.nibbles.elementAt(10), n2++));
        box4.add(new Row("MIDI CMP", this.nibbles.elementAt(9), n2++));
        box4.add(new Row("MIDI NG", this.nibbles.elementAt(14), n2++));
        box4.add(new Row("MIDI EQ", this.nibbles.elementAt(16), n2++));
        box4.add(new Row("MIDI BST", this.nibbles.elementAt(17), n2++));
        box4.add(new Row("MIDI MOD", this.nibbles.elementAt(11), n2++));
        box4.add(new Row("MIDI PIT", this.nibbles.elementAt(15), n2++));
        box4.add(new Row("MIDI DLY", this.nibbles.elementAt(12), n2++));
        box4.add(new Row("MIDI REV", this.nibbles.elementAt(13), n2++));
        box4.add(new Row("MIDI EXP", this.nibbles.elementAt(18), n2++));
        box4.add(new Row("Preset", null, 0));
        int n13 = ++n2;
        ++n2;
        box4.add(new Row("current preset", this.nibbles.elementAt(57), n13));
        while (n2 < n) {
            box4.add(new EmptyRow(false, n2++));
        }
        Box box5 = Box.createVerticalBox();
        box5.setBorder(Block.border);
        n2 = 0;
        Box box6 = Box.createHorizontalBox();
        box6.add(jComponent);
        box6.add(box);
        box6.add(box2);
        box6.add(box3);
        box6.add(box4);
        this.setLayout(new GridBagLayout());
        GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.insets = new Insets(0, 0, 0, 0);
        gridBagConstraints.fill = 2;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 0.0;
        this.mapin = new MidiMapIn(this);
        this.mapout = new MidiMapOut(this);
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        this.add((Component)box6, gridBagConstraints);
        ++gridBagConstraints.gridy;
        this.add((Component)this.mapin, gridBagConstraints);
        ++gridBagConstraints.gridy;
        this.add((Component)this.mapout, gridBagConstraints);
        this.closeButton.addActionListener(this);
        this.closeButton.setActionCommand("close");
        this.saveButton.addActionListener(this);
        this.saveButton.setActionCommand("save");
        this.saveXML.addActionListener(this);
        this.saveXML.setActionCommand("saveXML");
        this.saveXML.setEnabled(true);
        this.saveHTML.addActionListener(this);
        this.saveHTML.setActionCommand("saveHTML");
        this.saveHTML.setEnabled(true);
        this.refreshNovasList();
        this.send.setActionCommand("send");
        this.send.setEnabled(false);
        JPanel jPanel = new JPanel();
        jPanel.setOpaque(false);
        jPanel.add(this.closeButton);
        jPanel.add(this.saveButton);
        jPanel.add(this.saveXML);
        jPanel.add(this.saveHTML);
        jPanel.add(this.send);
        ++gridBagConstraints.gridy;
        gridBagConstraints.fill = 1;
        gridBagConstraints.anchor = 20;
        gridBagConstraints.weighty = 1.0;
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        this.add((Component)jPanel, gridBagConstraints);
    }

    int calculateChecksum() {
        return SystemDump.calculateChecksum(this.data);
    }

    static int calculateChecksum(byte[] byArray) {
        int n = 0;
        for (int i = 8; i <= 523; ++i) {
            n += byArray[i];
        }
        return n % 128;
    }

    public void actionPerformed(ActionEvent actionEvent) {
        if (actionEvent.getActionCommand().equals("close")) {
            this.closeTab();
        } else if (actionEvent.getActionCommand().equals("save")) {
            this.saveValues();
            this.saveSysEx();
        } else if (actionEvent.getActionCommand().equals("saveXML")) {
            this.saveValues();
            this.saveXML();
        } else if (actionEvent.getActionCommand().equals("saveHTML")) {
            this.saveValues();
            this.saveHTML();
        } else if (actionEvent.getActionCommand().equals("send") && this.send.getSelectedIndex() > 0) {
            Integer n = (Integer)this.send.getSelectedItem();
            this.data[4] = n.byteValue();
            this.saveValues();
            this.parent.send(this.data);
        }
    }

    public void writeBytesToFile(File file, byte[] byArray) {
        try {
            FileOutputStream fileOutputStream = new FileOutputStream(file);
            byte[] byArray2 = new byte[]{-16, 0, 32, 31, 127, 99, 69, 1, 31, 0, -9};
            fileOutputStream.write(byArray2);
            fileOutputStream.close();
            this.parent.logSilent("" + byArray2.length + " bytes written to " + file.getName());
        }
        catch (FileNotFoundException fileNotFoundException) {
            this.parent.log("FileNotFoundException : " + fileNotFoundException);
        }
        catch (IOException iOException) {
            this.parent.log("IOException : " + iOException);
        }
    }

    Nibble getNibble(int n) {
        return this.nibbles.elementAt(n);
    }

    int getNibbleCount() {
        return this.nibbles.size();
    }

    void closeTab() {
        this.parent.removeTab(this);
    }

    void refreshNovasList() {
        this.send.removeActionListener(this);
        this.send.removeAllItems();
        this.send.addItem("Send to Nova");
        List list = this.parent.getNovas();
        if (list != null) {
            for (int i = 0; i < list.size(); ++i) {
                this.send.addItem(list.get(i));
            }
        }
        this.send.addActionListener(this);
    }

    void saveSysEx() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileOutputStream fileOutputStream = new FileOutputStream(file);
                fileOutputStream.write(this.data);
                fileOutputStream.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveXML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toXML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveHTML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toHTML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveValues() {
        for (int i = 0; i < this.nibbles.size(); ++i) {
            this.getNibble(i).save(this.data);
        }
        this.data[524] = (byte)this.calculateChecksum();
    }

    String toHTML() {
        String string = "";
        string = string + "<HTML>\n";
        string = string + "<STYLE>TABLE.MIDI TD { font-size:10px } TABLE.MIDI {table-layout: fixed}</STYLE>\n";
        string = string + "  <BODY>\n";
        string = string + "    <H1>System Dump</H1>\n";
        string = string + "    <TABLE border=\"1\" cellpadding=\"10\" width=\"1000\">\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(0).toHTML();
        string = string + this.blocks.elementAt(1).toHTML();
        string = string + this.blocks.elementAt(4).toHTML();
        string = string + this.blocks.elementAt(7).toHTML();
        string = string + this.blocks.elementAt(6).toHTML();
        string = string + "      </TR>\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(2).toHTML();
        string = string + this.blocks.elementAt(5).toHTML();
        string = string + this.blocks.elementAt(8).toHTML();
        string = string + "      </TR>\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(3).toHTML();
        string = string + "      </TR>\n";
        string = string + "    </TABLE>\n";
        string = string + this.mapin.toHTML();
        string = string + this.mapout.toHTML();
        string = string + "  </BODY>\n";
        string = string + "</HTML>\n";
        return string;
    }

    public String toXML() {
        String string = "";
        string = string + "<system vendorId=\"00201F\" vendor=\"TC Electronic\" ";
        string = string + "modelId=\"63\" model=\"Nova System\">\n";
        for (int i = 0; i < this.blocks.size(); ++i) {
            Block block = this.blocks.elementAt(i);
            string = string + block.toXML();
        }
        string = string + this.mapin.toXML();
        string = string + this.mapout.toXML();
        string = string + "</system>";
        return string;
    }

    void setStandalone(boolean bl) {
        this.send.setEnabled(!bl);
    }

    public static boolean check(byte[] byArray) {
        boolean bl = true;
        if (bl && byArray.length != 526) {
            bl = false;
        }
        if (bl && byArray[0] != MidiInterface.aSystem[0]) {
            bl = false;
        }
        if (bl && byArray[1] != MidiInterface.aSystem[1]) {
            bl = false;
        }
        if (bl && byArray[2] != MidiInterface.aSystem[2]) {
            bl = false;
        }
        if (bl && byArray[3] != MidiInterface.aSystem[3]) {
            bl = false;
        }
        if (bl && byArray[5] != MidiInterface.aSystem[5]) {
            bl = false;
        }
        if (bl && byArray[6] != MidiInterface.aSystem[6]) {
            bl = false;
        }
        if (bl && byArray[7] != MidiInterface.aSystem[7]) {
            bl = false;
        }
        if (bl && byArray[525] != -9) {
            bl = false;
        }
        if (bl && byArray[524] != SystemDump.calculateChecksum(byArray)) {
            bl = false;
        }
        return bl;
    }

    public static void main(String[] stringArray) {
        if (stringArray.length != 1) {
            System.out.println("Syntax : java SystemDump <Sysex inputFile>");
        } else {
            File file = new File(stringArray[0]);
            try {
                byte[] byArray = NovaManager.getBytesFromFile(file);
                NovaManager novaManager = new NovaManager();
                novaManager.setEditable(true);
                SystemDump systemDump = new SystemDump(novaManager, stringArray[0], byArray);
                novaManager.add(systemDump);
            }
            catch (IOException iOException) {
                System.err.println("File not found : " + stringArray[1]);
            }
        }
    }
}
